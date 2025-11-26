using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using YouBoard.Models;
using YouBoard.Services;
using YouBoard.Utils;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private readonly AppVersionInfo appVersionInfo = new ();
        private readonly IYouTrackIssueClient issueClient;
        private readonly IDialogService dialogService;
        private ITabViewModel selectedTab;

        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(IYouTrackProjectClient youtrackProjectClient, IYouTrackIssueClient issueClient, IDialogService dialogService)
        {
            ProjectListViewModel.YouTrackProjectClient = youtrackProjectClient;
            ProjectListViewModel.ItemChosen += OpenProject;
            DynamicTabs.Add(ProjectListViewModel);

            this.issueClient = issueClient;
            this.dialogService = dialogService;
        }

        public ObservableCollection<ITabViewModel> DynamicTabs { get; set; } = new ();

        public ProjectListViewModel ProjectListViewModel { get; private set; } = new ();

        public ITabViewModel SelectedTab
        {
            get => selectedTab;
            set => SetProperty(ref selectedTab, value);
        }

        public string Title => appVersionInfo.Title;

        public DelegateCommand CloseTabCommand => new (() =>
        {
            Debug.Assert(SelectedTab != null, "SelectedTab should never be null.");
            if (SelectedTab is ProjectListViewModel)
            {
                // ProjectListViewModel は先頭に固定されているタブなので消さない。
                return;
            }

            var currentTab = SelectedTab as IssueListViewModel;
            DynamicTabs.Remove(currentTab);
        });

        public DelegateCommand<ITabViewModel> CloseTabByItemCommand => new (tab =>
        {
            if (tab is null)
            {
                return;
            }

            DynamicTabs.Remove(tab);
        });

        private void OpenProject(object sender, EventArgs e)
        {
            if (ProjectListViewModel.SelectedItem is not ProjectWrapper selectedProject)
            {
                return;
            }

            var exists = DynamicTabs.FirstOrDefault(t => t.Header == selectedProject.Name);
            if (exists != null)
            {
                SelectedTab = exists;
                return;
            }

            var issueTab = new IssueListViewModel(issueClient, selectedProject, dialogService);
            DynamicTabs.Add(issueTab);
            SelectedTab = issueTab;

            _ = issueTab.LoadIssuesAsync(5, 35);
        }
    }
}