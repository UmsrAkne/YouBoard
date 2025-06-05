using YouBoard.Models;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DesignProjectListViewModel : ProjectListViewModel
    {
        public DesignProjectListViewModel()
        {
            for (var i = 0; i < 15; i++)
            {
                ProjectWrappers.Add(new ProjectWrapper() { Name = $"Dummy Project {i}", ShortName = $"DP{i}", });
            }

            ProjectWrappers[1].IsFavorite = true;
            ProjectWrappers[2].IsFavorite = true;
        }
    }
}