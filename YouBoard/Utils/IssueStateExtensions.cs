using System;
using YouBoard.Models;

namespace YouBoard.Utils
{
    public static class IssueStateExtensions
    {
        public static string ToIssueStateName(this IssueState state)
        {
            return state switch
            {
                IssueState.Created => "未完了",
                IssueState.Complete => "完了",
                IssueState.Paused => "中断",
                IssueState.Working => "作業中",
                IssueState.Obsolete => "廃止",
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
            };
        }
    }
}