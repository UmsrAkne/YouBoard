using Prism.Mvvm;

namespace YouBoard.Models
{
    public class IssueSearchOption : BindableBase
    {
        private bool isSortByCreatedDate;
        private bool isSortByTitle;
        private bool isSortByType;

        public bool IsSortByCreatedDate
        {
            get => isSortByCreatedDate;
            set => SetProperty(ref isSortByCreatedDate, value);
        }

        public bool IsSortByTitle { get => isSortByTitle; set => SetProperty(ref isSortByTitle, value); }

        public bool IsSortByType { get => isSortByType; set => SetProperty(ref isSortByType, value); }
    }
}