using System;
using YouBoard.Models;

namespace YouBoard.Utils
{
    public static class IssueStateHelper
    {
        public static IssueState FromString(string name)
        {
            return name switch
            {
                "未完了" => IssueState.Created,
                "完了" => IssueState.Complete,
                "中断" => IssueState.Paused,
                "作業中" => IssueState.Working,
                "廃止" => IssueState.Obsolete,
                _ => throw new ArgumentException($"Invalid state name: {name}"),
            };
        }
    }
}