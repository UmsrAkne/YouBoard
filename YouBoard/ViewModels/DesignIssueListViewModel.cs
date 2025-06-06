using YouBoard.Models;

namespace YouBoard.ViewModels
{
    public class DesignIssueListViewModel : IssueListViewModel
    {
        public DesignIssueListViewModel()
        {
            for (var i = 0; i < 15; i++)
            {
                IssueWrappers.Add(new IssueWrapper() { Title = $"Dummy Issue {i}", Id = $"Id-{i}", });
            }
        }
    }
}