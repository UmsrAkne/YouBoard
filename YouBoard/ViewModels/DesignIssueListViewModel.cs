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

            IssueWrappers[0].IsComplete = true;

            // IssueListView 内で複雑なアニメーション（Storyboard）を使用しているため、
            // XAML デザイナーが正しく解決できず、IssueWrapper のプレビュー全体が無効化される問題がある。
            // IsRunning 状態での動作プレビューはできなくなるが、要素全体が非表示になるのは避けたい。
            // そのため、以下の設定はコメントアウトしておく。
            // IssueWrappers[1].WorkTimer.IsRunning = true;
            IssueWrappers[2].IsExpanded = true;
            IssueWrappers[2].Comments.Add(new IssueCommentWrapper() { Text = "Comment1", });
            IssueWrappers[2].Comments.Add(new IssueCommentWrapper() { Text = "Comment2", });
        }

        public new bool IsDesignInstance => true;
    }
}