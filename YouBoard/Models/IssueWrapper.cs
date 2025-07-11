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

        public string Id { get => id; set => SetProperty(ref id, value); }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public IssueState State { get => state; set => SetProperty(ref state, value); }

        public bool IsComplete { get => isComplete; set => SetProperty(ref isComplete, value); }

        public string Description { get => description; set => SetProperty(ref description, value); }

        public WorkTimer WorkTimer { get; } = new ();

        public bool IsExpanded { get => isExpanded; set => SetProperty(ref isExpanded, value); }

        public string PendingComment { get => pendingComment; set => SetProperty(ref pendingComment, value); }

        public IssueType Type { get => type; set => SetProperty(ref type, value); }

        public ObservableCollection<IssueCommentWrapper> Comments { get; set; } = new ();

        public DateTime Created { get; set; }
    }
}