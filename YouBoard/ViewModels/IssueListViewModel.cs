using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using YouBoard.Models;
using YouBoard.Services;

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
        private readonly string[] spinnerFrames = new[] { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏", };

        private IssueWrapper pendingIssue = new ();
        private string title = string.Empty;
        private int spinnerIndex = 0;
        private object selectedItem;

        public IssueListViewModel()
        {
        }

        public IssueListViewModel(IYouTrackIssueClient client, ProjectWrapper project, IDialogService dialogService)
        {
            var projectName = project.Name;
            projectShortName = project.ShortName;
            this.client = client;

            Header = projectName;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += RefreshWindowTitle;

            this.dialogService = dialogService;
        }

        public event EventHandler ItemChosen;

        public string TabType { get; set; } = "IssueList";

        public string Header { get; set; }

        public object SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public ObservableCollection<IssueWrapper> IssueWrappers { get; set; } = new ();

        public IssueWrapper PendingIssue { get => pendingIssue; set => SetProperty(ref pendingIssue, value); }

        public AsyncRelayCommand CreateIssueCommand => new (async () =>
        {
            if (string.IsNullOrWhiteSpace(PendingIssue.Title))
            {
                return;
            }

            var newIssue = await client.CreateIssueAsync(projectShortName, PendingIssue);
            IssueWrappers.Insert(0, newIssue);
            PendingIssue = new IssueWrapper();
            UpdateTimingStatus();
        });

        public AsyncRelayCommand<IssueWrapper> MarkAsCompleteIssueCommand => new (async (param) =>
        {
            if (param == null)
            {
                return;
            }

            await client.MarkAsCompleteAsync(param);
            UpdateTimingStatus();
        });

        public AsyncRelayCommand<IssueWrapper> ToggleIssueStateCommandAsync => new (async (param) =>
        {
            if (param == null)
            {
                return;
            }

            await client.ToggleIssueWorkStateAsync(param);
            if (param.State == IssueState.Working)
            {
                param.WorkTimer.Start();
            }
            else
            {
                param.WorkTimer.Pause();
            }

            UpdateTimingStatus();
        });

        public AsyncRelayCommand<IssueWrapper> AddCommentCommandAsync => new (async (param) =>
        {
            if (param == null || string.IsNullOrWhiteSpace(param.PendingComment))
            {
                return;
            }

            await client.AddCommentAsync(param, param.PendingComment);
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
            };
        });

        public AsyncRelayCommand GenerateNextIssueCopyCommand => new (async () =>
        {
            if (SelectedItem is not IssueWrapper item)
            {
                return;
            }

            // 正規表現で "title[001]" 形式をパース
            var match = Regex.Match(item.Title, @"^(.*)\[(\d+)\]$");
            string baseTitle;

            if (match.Success)
            {
                baseTitle = match.Groups[1].Value;
            }
            else
            {
                // フォーマットに沿っていない場合は title[1] を初期化
                baseTitle = item.Title;
                item.Title = $"{baseTitle}[1]";
            }

            // 同じ baseTitle を含むチケットを取得
            var issues = await client.GetIssuesByProjectAsync(projectShortName, baseTitle);

            // 既存の中から最大の番号を探す
            var maxNumber = issues
                .Select(i => Regex.Match(i.Title, @$"^{Regex.Escape(baseTitle)}\[(\d+)\]$"))
                .Where(m => m.Success)
                .Select(m => int.Parse(m.Groups[1].Value))
                .DefaultIfEmpty(0)
                .Max();

            var nextNumber = maxNumber + 1;

            // PendingIssue を生成
            PendingIssue = new IssueWrapper
            {
                Title = $"{baseTitle}[{nextNumber}]",
                State = item.State,
                Description = item.Description,
                Type = item.Type,
            };
        });

        public DelegateCommand<IssueWrapper> ApplySelectionCommand => new ((param) =>
        {
            SelectedItem = param;
        });

        public DelegateCommand ShowBulkCreateIssuePageCommand => new DelegateCommand(() =>
        {
            try
            {
                dialogService.ShowDialog("BulkCreateIssuePage", null, _ => { });
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
            // Step 1: 最新の initialCount 件を読み込み
            var initialIssues = await client.GetIssuesByProjectAsync(projectShortName, initialCount);
            IssueWrappers.Clear();
            foreach (var issue in initialIssues)
            {
                IssueWrappers.Add(issue);
            }

            // Step 2: 残りの additionalCount 件を非同期で追加読み込み
            _ = Task.Run(async () =>
            {
                var additionalIssues =
                    await client.GetIssuesByProjectAsync(projectShortName, additionalCount, initialCount);

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
            Title = $"[{totalMin}m {frame}] {workingIssue.Title}";
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
    }
}