using Prism.Mvvm;

namespace YouBoard.Models
{
    public class IssueWrapper : BindableBase
    {
        private string id = string.Empty;
        private string title;

        public string Id { get => id; set => SetProperty(ref id, value); }

        public string Title { get => title; set => SetProperty(ref title, value); }
    }
}