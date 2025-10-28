using Prism.Mvvm;

namespace YouBoard.Models
{
    public class IssueSearchOption : BindableBase
    {
        private bool isSortByCreatedDate;
        private bool isSortByTitle;
        private bool isSortByType;
        private bool isAscending;
        private int limit;
        private int offset;
        private string searchPattern = string.Empty;
        private string projectShortName = string.Empty;

        public bool IsSortByCreatedDate
        {
            get => isSortByCreatedDate;
            set => SetProperty(ref isSortByCreatedDate, value);
        }

        public bool IsSortByTitle { get => isSortByTitle; set => SetProperty(ref isSortByTitle, value); }

        public bool IsSortByType { get => isSortByType; set => SetProperty(ref isSortByType, value); }

        public bool IsAscending { get => isAscending; set => SetProperty(ref isAscending, value); }

        public int Limit { get => limit; set => SetProperty(ref limit, value); }

        public int Offset { get => offset; set => SetProperty(ref offset, value); }

        public string SearchPattern { get => searchPattern; set => SetProperty(ref searchPattern, value); }

        public string ProjectShortName
        {
            get => projectShortName;
            set => SetProperty(ref projectShortName, value);
        }
    }
}