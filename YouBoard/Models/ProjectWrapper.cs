using System.Text.Json.Serialization;
using Prism.Mvvm;
using YouTrackSharp.Projects;

namespace YouBoard.Models
{
    public class ProjectWrapper : BindableBase
    {
        private string name = string.Empty;
        private string shortName = string.Empty;
        private bool isSelected;
        private ProjectProfile projectProfile;

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

        public ProjectProfile ProjectProfile
        {
            get => projectProfile ??= new ProjectProfile();
            set => projectProfile = value;
        }

        [JsonIgnore]
        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }
    }
}