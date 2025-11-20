using System;
using Prism.Mvvm;

namespace YouBoard.Models
{
    public class ProjectProfile : BindableBase
    {
        private bool isFavorite;
        private bool isArchive;

        // Defaults for new issues in this project
        private string defaultIssueDescription = string.Empty;
        private TimeSpan defaultEstimatedDuration = TimeSpan.Zero;
        private IssueType defaultIssueType = IssueType.Todo;

        // Default sort state when opening the project
        private bool sortByCreatedDate = true;
        private bool sortByTitle;
        private bool sortByType;
        private bool isAscending;
        private bool isEntryNoInputEnabled;

        public bool IsFavorite { get => isFavorite; set => SetProperty(ref isFavorite, value); }

        public bool IsArchive { get => isArchive; set => SetProperty(ref isArchive, value); }

        public string DefaultIssueDescription
        {
            get => defaultIssueDescription;
            set => SetProperty(ref defaultIssueDescription, value);
        }

        public TimeSpan DefaultEstimatedDuration
        {
            get => defaultEstimatedDuration;
            set => SetProperty(ref defaultEstimatedDuration, value);
        }

        public IssueType DefaultIssueType
        {
            get => defaultIssueType;
            set => SetProperty(ref defaultIssueType, value);
        }

        // Sort state flags (mutually exclusive for field selection)
        public bool SortByCreatedDate
        {
            get => sortByCreatedDate;
            set
            {
                if (SetProperty(ref sortByCreatedDate, value) && value)
                {
                    // Ensure exclusivity
                    SortByTitle = false;
                    SortByType = false;
                }
            }
        }

        public bool SortByTitle
        {
            get => sortByTitle;
            set
            {
                if (SetProperty(ref sortByTitle, value) && value)
                {
                    SortByCreatedDate = false;
                    SortByType = false;
                }
            }
        }

        public bool SortByType
        {
            get => sortByType;
            set
            {
                if (SetProperty(ref sortByType, value) && value)
                {
                    SortByCreatedDate = false;
                    SortByTitle = false;
                }
            }
        }

        public bool IsAscending
        {
            get => isAscending;
            set => SetProperty(ref isAscending, value);
        }

        public bool IsEntryNoInputEnabled
        {
            get => isEntryNoInputEnabled;
            set => SetProperty(ref isEntryNoInputEnabled, value);
        }
    }
}