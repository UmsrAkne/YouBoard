using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

        public async Task LoadAsync()
        {
            var issues = await client.GetIssuesByProjectAsync(projectShortName);
            IssueWrappers.Clear();
            foreach (var issue in issues)
            {
                IssueWrappers.Add(issue);
            }
        }
    }
}