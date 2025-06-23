using System.Collections.Generic;
using YouBoard.Models;

namespace YouBoard.Utils
{
    public static class IssueTypeHelper
    {
        public static string ToIssueTypeName(IssueType value)
        {
            return value switch
            {
                IssueType.Feature => "機能",
                IssueType.Appearance => "外観",
                IssueType.Test => "テスト",
                IssueType.Todo => "タスク",
                IssueType.Bug => "バグ",
                _ => string.Empty,
            };
        }

        public static IssueType FromString(string description)
        {
            return description switch
            {
                "機能" => IssueType.Feature,
                "外観" => IssueType.Appearance,
                "テスト" => IssueType.Test,
                "タスク" => IssueType.Todo,
                "バグ" => IssueType.Bug,
                _ => IssueType.Feature,
            };
        }

        public static bool CanConvert(string text)
        {
            return new HashSet<string> { "機能", "外観", "テスト", "タスク", "バグ", }.Contains(text);
        }
    }
}