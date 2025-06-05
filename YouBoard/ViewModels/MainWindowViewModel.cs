using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;
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
        private ITabViewModel selectedTab;

        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(IYouTrackProjectClient youtrackProjectClient, IYouTrackIssueClient issueClient)
        {
            ProjectListViewModel.YouTrackProjectClient = youtrackProjectClient;
            ProjectListViewModel.ItemChosen += OpenProject;
            DynamicTabs.Add(ProjectListViewModel);

            this.issueClient = issueClient;
        }

        public ObservableCollection<ITabViewModel> DynamicTabs { get; set; } = new ();

        public ProjectListViewModel ProjectListViewModel { get; private set; } = new ();

        public ITabViewModel SelectedTab { get => selectedTab; set => SetProperty(ref selectedTab, value); }

        public string Title => appVersionInfo.Title;

        private void OpenProject(object sender, EventArgs e)
        {
            if (ProjectListViewModel.SelectedItem is not ProjectWrapper selectedProject)
            {
                return;
            }

            var issueTab = new IssueListViewModel(issueClient, selectedProject);
            DynamicTabs.Add(issueTab);
            SelectedTab = issueTab;

            _ = issueTab.LoadAsync();
        }
    }
}