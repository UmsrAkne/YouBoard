using System.Collections.ObjectModel;
using YouBoard.Models;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DesignProjectListViewModel : ProjectListViewModel
    {
        private ObservableCollection<ProjectWrapper> projects = new ();

        public DesignProjectListViewModel()
        {
            for (var i = 0; i < 15; i++)
            {
                Projects.Add(new ProjectWrapper() { Name = $"Dummy Project {i}", ShortName = $"DP{i}", });
            }

            Projects[1].IsFavorite = true;
            Projects[2].IsFavorite = true;
        }

        public ObservableCollection<ProjectWrapper> Projects
        {
            get => projects;
            set => SetProperty(ref projects, value);
        }
    }
}