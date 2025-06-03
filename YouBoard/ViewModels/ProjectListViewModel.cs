using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Mvvm;
using YouBoard.Models;
using YouBoard.Services;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProjectListViewModel : BindableBase, ITabViewModel
    {
        private ObservableCollection<ProjectWrapper> projectWrappers = new ();

        public IYouTrackProjectClient YouTrackProjectClient { get; set; }

        public ObservableCollection<ProjectWrapper> ProjectWrappers
        {
            get => projectWrappers;
            set => SetProperty(ref projectWrappers, value);
        }

        public string TabType { get; set; } = "ProjectList";

        public string Header { get; set; } = "Projects";

        public AsyncRelayCommand LoadProjectsCommand => new AsyncRelayCommand(async () =>
        {
            if (YouTrackProjectClient == null)
            {
                return;
            }

            var p = await YouTrackProjectClient.GetProjectsAsync();
            ProjectWrappers = new ObservableCollection<ProjectWrapper>(p);
        });
    }
}