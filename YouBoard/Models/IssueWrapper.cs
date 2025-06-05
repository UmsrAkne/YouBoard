using Prism.Mvvm;

namespace YouBoard.Models
{
    public class IssueWrapper : BindableBase
    {
        private string id = string.Empty;
        private string title;
        private string status;
        private bool isComplete;

        public string Id { get => id; set => SetProperty(ref id, value); }

        public string Title { get => title; set => SetProperty(ref title, value); }

        public string Status { get => status; set => SetProperty(ref status, value); }

        public bool IsComplete { get => isComplete; set => SetProperty(ref isComplete, value); }
    }
}