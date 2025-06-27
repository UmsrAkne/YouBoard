using YouBoard.Models;

namespace YouBoard.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DesignIssueListViewModel : IssueListViewModel
    {
        public DesignIssueListViewModel()
        {
            for (var i = 0; i < 15; i++)
            {
                IssueWrappers.Add(new IssueWrapper() { Title = $"Dummy Issue {i}", Id = $"Id-{i}", });
            }

            IssueWrappers[1].WorkTimer.IsRunning = true;
            IssueWrappers[2].IsExpanded = true;
            IssueWrappers[2].Comments.Add(new IssueCommentWrapper() { Text = "Comment1", });
            IssueWrappers[2].Comments.Add(new IssueCommentWrapper() { Text = "Comment2", });
        }

        public bool IsDesignInstance => true;
    }
}