using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using YouBoard.Utils;

namespace YouBoard.Models
{
    public class IssueWrapper : BindableBase
    {
        private string id = string.Empty;
        private string title;
        private IssueState state;
        private bool isComplete;
        private string description;
        private bool isExpanded;
        private string pendingComment;
        private IssueType type;
        private TimeSpan estimatedDuration = TimeSpan.Zero;
        private WorkTimer workTimer = new ();
        private TimeSpan elapsedDuration;
        private ObservableCollection<IssueCommentWrapper> comments = new ();

        public string Id { get => id; set => SetProperty(ref id, value); }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public IssueState State { get => state; set => SetProperty(ref state, value); }

        public bool IsComplete { get => isComplete; set => SetProperty(ref isComplete, value); }

        public string Description { get => description; set => SetProperty(ref description, value); }

        public WorkTimer WorkTimer { get => workTimer; set => SetProperty(ref workTimer, value); }

        public bool IsExpanded { get => isExpanded; set => SetProperty(ref isExpanded, value); }

        public string PendingComment { get => pendingComment; set => SetProperty(ref pendingComment, value); }

        public IssueType Type { get => type; set => SetProperty(ref type, value); }

        public ObservableCollection<IssueCommentWrapper> Comments
        {
            get => comments;
            set => SetProperty(ref comments, value);
        }

        public DateTime Created { get; set; }

        public TimeSpan EstimatedDuration
        {
            get => estimatedDuration;
            set => SetProperty(ref estimatedDuration, value);
        }

        public TimeSpan ElapsedDuration
        {
            get => elapsedDuration;
            set => SetProperty(ref elapsedDuration, value);
        }
    }
}