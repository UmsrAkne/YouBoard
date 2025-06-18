using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using Prism.Mvvm;
using YouBoard.Models;
using YouBoard.Services;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IssueListViewModel : BindableBase, ITabViewModel
    {
        private readonly IYouTrackIssueClient client;
        private readonly string projectName = string.Empty;
        private readonly string projectShortName = string.Empty;
        private IssueWrapper pendingIssue = new ();

        public IssueListViewModel()
        {
        }

        public IssueListViewModel(IYouTrackIssueClient client, ProjectWrapper project)
        {
            projectName = project.Name;
            projectShortName = project.ShortName;
            this.client = client;

            Header = projectName;
        }

        public event EventHandler ItemChosen;

        public string TabType { get; set; } = "IssueList";

        public string Header { get; set; }

        public object SelectedItem { get; set; }

        public ObservableCollection<IssueWrapper> IssueWrappers { get; set; } = new ();

        public IssueWrapper PendingIssue { get => pendingIssue; set => SetProperty(ref pendingIssue, value); }

        public AsyncRelayCommand CreateIssueCommand => new (async () =>
        {
            var newIssue = await client.CreateIssueAsync(projectShortName, PendingIssue);
            IssueWrappers.Insert(0, newIssue);
            pendingIssue = new IssueWrapper();
        });

        public AsyncRelayCommand<IssueWrapper> MarkAsCompleteIssueCommand => new (async (param) =>
        {
            if (param == null)
            {
                return;
            }

            await client.MarkAsCompleteAsync(param);
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
        }
    }
}