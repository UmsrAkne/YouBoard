using System.Collections.ObjectModel;
using Prism.Mvvm;
using YouBoard.Models;
using YouBoard.Services;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProjectListViewModel : BindableBase, ITabViewModel
    {
        public IYouTrackProjectClient YouTrackProjectClient { get; set; }

        public ObservableCollection<ProjectWrapper> ProjectWrappers { get; set; } = new ();

        public string TabType { get; set; } = "ProjectList";

        public string Header { get; set; } = "Projects";
    }
}