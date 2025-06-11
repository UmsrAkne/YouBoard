using Prism.Mvvm;

namespace YouBoard.Models
{
    public class IssueWrapper : BindableBase
    {
        private string id = string.Empty;
        private string title;
        private IssueState state;
        private bool isComplete;
        private string description;

        public string Id { get => id; set => SetProperty(ref id, value); }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public IssueState State { get => state; set => SetProperty(ref state, value); }

        public bool IsComplete { get => isComplete; set => SetProperty(ref isComplete, value); }

        public string Description { get => description; set => SetProperty(ref description, value); }
    }
}