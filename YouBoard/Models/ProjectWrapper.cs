using Prism.Mvvm;
using YouTrackSharp.Projects;

namespace YouBoard.Models
{
    public class ProjectWrapper : BindableBase
    {
        private string name = string.Empty;
        private string shortName = string.Empty;
        private bool isFavorite;

        public ProjectWrapper()
        {
        }

        public ProjectWrapper(Project project)
        {
            Name = project.Name;
            ShortName = project.ShortName;
        }

        public string Name { get => name; set => SetProperty(ref name, value); }

        public string ShortName { get => shortName; set => SetProperty(ref shortName, value); }

        public bool IsFavorite { get => isFavorite; set => SetProperty(ref isFavorite, value); }
    }
}