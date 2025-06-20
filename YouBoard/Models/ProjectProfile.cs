using Prism.Mvvm;

namespace YouBoard.Models
{
    public class ProjectProfile : BindableBase
    {
        private bool isFavorite;
        private bool isArchive;

        public bool IsFavorite { get => isFavorite; set => SetProperty(ref isFavorite, value); }

        public bool IsArchive { get => isArchive; set => SetProperty(ref isArchive, value); }
    }
}