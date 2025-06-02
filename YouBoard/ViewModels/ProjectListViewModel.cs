using System.Collections.ObjectModel;
using Prism.Mvvm;
using YouBoard.Models;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProjectListViewModel : BindableBase, ITabViewModel
    {
        public ObservableCollection<ProjectWrapper> ProjectWrappers { get; set; } = new ();

        public string Header { get; set; } = "Projects";
    }
}