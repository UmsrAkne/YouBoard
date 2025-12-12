using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using YouBoard.Models;
using YouBoard.Services;
using YouBoard.Utils;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IssueListViewModel : BindableBase, ITabViewModel
    {
        private readonly IYouTrackIssueClient client;
        private readonly List<IssueWrapper> timingItems = new ();
        private readonly string projectShortName = string.Empty;
        private readonly DispatcherTimer timer = new ();
        private readonly IDialogService dialogService;
        private readonly string[] spinnerFrames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏", };
        private readonly ProjectWrapper projectWrapper;

        private IssueWrapper pendingIssue;
        private string title = string.Empty;
        private int spinnerIndex;
        private object selectedItem;
        private bool isIssueCreating;
        private ProjectWrapper projectWrapper1;
        private IList selectedIssues = new List<IssueWrapper>();

        public IssueListViewModel()
        {
        }

        public IssueListViewModel(IYouTrackIssueClient client, ProjectWrapper project, IDialogService dialogService)
        {
            var projectName = project.Name;
            projectShortName = project.ShortName;
            this.client = client;
            ProjectWrapper = project;

            Header = projectName;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += RefreshWindowTitle;

            ApplyProjectDefaultsToPendingIssue();

            IssueSearchOption.ProjectShortName = projectShortName;
            IssueSearchOption.Limit = 40;
            IssueSearchOption.IsSortByCreatedDate = true;

            this.dialogService = dialogService;
        }

        public event EventHandler ItemChosen;

        public string TabType { get; set; } = "IssueList";

        public string Header { get; set; }

        public object SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public ObservableCollection<IssueWrapper> IssueWrappers { get; set; } = new ();

        public IssueWrapper PendingIssue { get => pendingIssue; set => SetProperty(ref pendingIssue, value); }

        public bool IsIssueCreating { get => isIssueCreating; set => SetProperty(ref isIssueCreating, value); }

        public IssueSearchOption IssueSearchOption { get; set; } = new IssueSearchOption();

        public IList SelectedIssues
        {
            get => selectedIssues;
            set => SetProperty(ref selectedIssues, value);
        }

        public bool IsDesignInstance => false;

        public ProjectWrapper ProjectWrapper
        {
            get => projectWrapper1;
            private set => SetProperty(ref projectWrapper1, value);
        }

        public AsyncRelayCommand CreateIssueCommand => new (async () =>
        {
            if (string.IsNullOrWhiteSpace(PendingIssue.Title))
            {
                return;
            }

            IsIssueCreating = true;
            try
            {
                var newIssue = await client.CreateIssueAsync(projectShortName, PendingIssue);
                IssueWrappers.Insert(0, newIssue);
                ApplyProjectDefaultsToPendingIssue();
                UpdateTimingStatus();
            }
            finally
            {
                IsIssueCreating = false;
            }
        }, () => !IsIssueCreating);

        public AsyncRelayCommand<IssueWrapper> MarkAsCompleteIssueCommand => new (async (param) =>
        {
            if (param == null)
            {
                return;
            }

            param.IsComplete = true;
            param.State = IssueState.Complete;
            if (param.WorkTimer.IsRunning)
            {
                param.ElapsedDuration = param.WorkTimer.Elapsed;
            }

            await client.MarkAsCompleteAsync(param);
            await client.AddCommentAsync(param, $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} Complete");

            UpdateTimingStatus();
        });

        // ReSharper disable once UnusedMember.Global
        public AsyncRelayCommand<IssueWrapper> ToObsoleteCommandAsync => new (async (param) =>
        {
            if (param == null || param.State == IssueState.Obsolete)
            {
                return;
            }

            if (param.State == IssueState.Working)
            {
                await client.AddWorkingDurationAsync(param, param.WorkTimer.Elapsed, "課題を廃止しました");
                param.ElapsedDuration += param.WorkTimer.Elapsed;
                param.WorkTimer.Reset();
            }

            param.State = IssueState.Obsolete;

            // トグルした状態をサーバーに通知する。
            await client.PostIssueStateAsync(param);
            UpdateTimingStatus();
        });

        public AsyncRelayCommand<IssueWrapper> ToggleIssueStateCommandAsync => new (async (param) =>
        {
            if (param == null)
            {
                return;
            }

            if (param.State == IssueState.Working)
            {
                await client.AddWorkingDurationAsync(param, param.WorkTimer.Elapsed, string.Empty);
                param.ElapsedDuration += param.WorkTimer.Elapsed;
                param.WorkTimer.Reset();
            }

            // Complete, Obsolete の状態ではこのコマンドは実行不可のはずなので、万一それが来た場合は例外をスローする。
            param.State = param.State switch
            {
                IssueState.Created => IssueState.Working,
                IssueState.Paused => IssueState.Working,
                IssueState.Working => IssueState.Created,
                IssueState.Complete or IssueState.Obsolete => throw new InvalidOperationException($"Cannot toggle state from {param.State}."),
                _ => throw new InvalidOperationException($"Unexpected state: {param.State}"),
            };

            // トグルした状態をサーバーに通知する。
            await client.PostIssueStateAsync(param);

            // 課題の状態に合わせて WorkTimer を On, Off
            if (param.State == IssueState.Working)
            {
                param.WorkTimer.Start();
            }
            else
            {
                param.WorkTimer.Pause();
            }

            if (param.ElapsedDuration != TimeSpan.Zero)
            {
                param.State = IssueState.Paused;
            }

            UpdateTimingStatus();
        });

        public AsyncRelayCommand<IssueWrapper> AddCommentCommandAsync => new (async (param) =>
        {
            if (param == null || string.IsNullOrWhiteSpace(param.PendingComment))
            {
                return;
            }

            var posted = await client.AddCommentAsync(param, param.PendingComment);
            param.Comments.Add(posted);
            param.Comments = new ObservableCollection<IssueCommentWrapper>(param.Comments.OrderByDescending(c => c.CreatedAt));

            param.PendingComment = string.Empty;
        });

        public AsyncRelayCommand<IssueWrapper> AddWorkingDurationCommandAsync => new (async (param) =>
        {
            if (param is not { IsComplete: true, })
            {
                return;
            }

            var duration = param.WorkTimer.Elapsed;
            param.WorkTimer.IsRunning = false;

            await client.AddWorkingDurationAsync(param, duration, string.Empty);
        });

        public AsyncRelayCommand<IssueWrapper> LoadCommentsCommandAsync => new (async (param) =>
        {
            if (param == null)
            {
                return;
            }

            await client.LoadCommentsAsync(param);
        });

        public DelegateCommand CopyIssueCommand => new DelegateCommand(() =>
        {
            if (SelectedItem is not IssueWrapper item)
            {
                return;
            }

            PendingIssue = new IssueWrapper
            {
                Title = item.Title,
                State = item.State,
                Description = item.Description,
                Type = item.Type,
                EstimatedDuration = item.EstimatedDuration,
            };
        });

        public AsyncRelayCommand GenerateNextIssueCopyCommand => new (async () =>
        {
            if (SelectedItem is not IssueWrapper item)
            {
                return;
            }

            // 全ての Issue を分割して取得。
            // 全取得は取得数制限に引っかかる懸念があるのでNG。
            const int pageSize = 100;
            var offset = 0;
            var allIssues = new List<IssueWrapper>();
            var minimumNumber = item.EntryNo;

            while (true)
            {
                var opt = new IssueSearchOption
                {
                    ProjectShortName = projectShortName,
                    Limit = pageSize,
                    Offset = offset,
                    IsSortByCreatedDate = true,
                    MinEntryNo = minimumNumber,
                };

                var page = await client.GetIssuesByProjectAsync(opt);

                if (page == null || page.Count == 0)
                {
                    break; // もう取れるデータはない
                }

                allIssues.AddRange(page);
                offset += pageSize;
            }

            var maxNumber = IssueSelector.GetMaxEntryNoForSameTitle(allIssues, item);

            // PendingIssue を生成
            PendingIssue = new IssueWrapper
            {
                Title = item.Title,
                State = item.State,
                Description = item.Description,
                Type = item.Type,
                EstimatedDuration = item.EstimatedDuration,
                EntryNo = maxNumber + 1,
            };
        });

        public DelegateCommand CopyToClipboardCommand => new DelegateCommand(() =>
        {
            // 選択済み Issue をキャスト・並び替え。
            var items = SelectedIssues
                .Cast<IssueWrapper>()
                .OrderBy(i => i.Id)
                .ToList();

            if (items.Count == 0)
            {
                return;
            }

            // テンプレートを1回だけ取得
            var templateName = string.IsNullOrWhiteSpace(App.AppSettings.TemplateFileName)
                ? "detail"
                : App.AppSettings.TemplateFileName;

            var template = YamlTemplateProvider.GetTemplate(templateName);

            var builder = new StringBuilder();
            foreach (var issue in items)
            {
                builder.AppendLine(YamlTemplateRenderer.Render(template, issue.ToDictionary()));
            }

            Clipboard.SetText(builder.ToString());
        });

        public AsyncRelayCommand ToggleResolvedFilterCommand => new (async () =>
        {
            IssueSearchOption.IsOnlyUnresolved = !IssueSearchOption.IsOnlyUnresolved;
            await ReloadIssuesCommand.ExecuteAsync(null);
        });

        public AsyncRelayCommand ReloadIssuesCommand => new (async () =>
        {
            await LoadIssuesAsync(5, IssueSearchOption.Limit);
        });

        public DelegateCommand<IssueWrapper> ApplySelectionCommand => new ((param) =>
        {
            SelectedItem = param;
        });

        public DelegateCommand ShowBulkCreateIssuePageCommand => new DelegateCommand(() =>
        {
            try
            {
                var parameters = new DialogParameters
                {
                    { "client", client },
                    { "projectShortName", projectShortName },
                };

                dialogService.ShowDialog("BulkCreateIssuePage", parameters, result =>
                {
                    if (result is { Result: ButtonResult.OK, })
                    {
                        _ = ReloadIssuesCommand.ExecuteAsync(null);
                    }
                });
            }
            catch
            {
                // Ignore any dialog exceptions to avoid breaking startup in design-time or tests.
            }
        });

        public DelegateCommand ShowSettingsPageCommand => new DelegateCommand(() =>
        {
            try
            {
                dialogService.ShowDialog("SettingsPage");
            }
            catch
            {
                // Ignore any dialog exceptions to avoid breaking startup in design-time or tests.
            }
        });

        /// <summary>
        /// Loads a specified number of issues from the server and adds them to IssueWrappers.
        /// </summary>
        /// <param name="initialCount">The number of issues to load initially. Intended to be a small number.</param>
        /// <param name="additionalCount">The number of additional issues to load asynchronously after the initial load completes.</param>
        /// <returns>A task that represents the asynchronous operation of loading issues.</returns>
        public async Task LoadIssuesAsync(int initialCount, int additionalCount)
        {
            var limitOrigin = IssueSearchOption.Limit;
            var offsetOrigin = IssueSearchOption.Offset;

            IssueSearchOption.Limit = initialCount;
            IssueSearchOption.Offset = 0;

            var oldList = IssueWrappers.ToList();

            // Step 1: 最新の initialCount 件を読み込み
            var initialIssues = await client.GetIssuesByProjectAsync(IssueSearchOption);
            SetRunningWorkTimer(oldList, initialIssues);
            IssueWrappers.Clear();
            foreach (var issue in initialIssues)
            {
                IssueWrappers.Add(issue);
            }

            // Step 2: 残りの additionalCount 件を非同期で追加読み込み
            _ = Task.Run(async () =>
            {
                IssueSearchOption.Limit = additionalCount;
                IssueSearchOption.Offset = initialCount;
                var additionalIssues =
                    await client.GetIssuesByProjectAsync(IssueSearchOption);

                SetRunningWorkTimer(oldList, additionalIssues);
                foreach (var issue in additionalIssues)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        IssueWrappers.Add(issue);
                    });

                    await Task.Delay(50); // 課題の追加演出。指定時間刻みで一件ずつアイテムが追加されていく。
                }
            });

            UpdateTimingStatus();
            IssueSearchOption.Limit = limitOrigin;
            IssueSearchOption.Offset = offsetOrigin;

            void SetRunningWorkTimer(List<IssueWrapper> oldIssues, List<IssueWrapper> newIssues)
            {
                var watchingIssues = oldIssues.Where(i => i.WorkTimer.IsRunning).ToList();
                if (!watchingIssues.Any())
                {
                    return;
                }

                foreach (var watchingIssue in watchingIssues)
                {
                    var ni = newIssues.FirstOrDefault(i => i.Id == watchingIssue.Id);
                    if (ni != null)
                    {
                        ni.WorkTimer = watchingIssue.WorkTimer;
                    }
                }
            }
        }

        private void RefreshWindowTitle(object sender, EventArgs e)
        {
            var workingIssue = IssueWrappers.FirstOrDefault(i => i.State == IssueState.Working);
            if (workingIssue == null)
            {
                return;
            }

            var totalMin = (int)workingIssue.WorkTimer.Elapsed.TotalMinutes;
            var frame = spinnerFrames[spinnerIndex++ % spinnerFrames.Length];
            var entryNo = workingIssue.EntryNo;
            Title = $"[{totalMin}m {frame}] {workingIssue.Title}" + (entryNo != 0 ? $" #{entryNo}" : string.Empty);
        }

        private void UpdateTimingStatus()
        {
            var targets = IssueWrappers.Where(i => i.State == IssueState.Working);
            timingItems.Clear();
            timingItems.AddRange(targets);

            if (timingItems.Count != 0)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
                Title = Header;
            }
        }

        private void ApplyProjectDefaultsToPendingIssue()
        {
            var profile = ProjectWrapper.ProjectProfile;

            PendingIssue = new IssueWrapper()
            {
                Description = profile.DefaultIssueDescription,
                EstimatedDuration = profile.DefaultEstimatedDuration,
                Type = profile.DefaultIssueType,
            };
        }
    }
}