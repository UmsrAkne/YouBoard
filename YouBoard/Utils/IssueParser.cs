using System;
using System.Collections.Generic;
using System.Linq;
using YouBoard.Models;

namespace YouBoard.Utils
{
    public static class IssueParser
    {
        private readonly static Dictionary<string, IssueType> IssueTypeMap = new (StringComparer.OrdinalIgnoreCase)
        {
            { "feature", IssueType.Feature },
            { "feat", IssueType.Feature },
            { "todo", IssueType.Todo },
            { "appearance", IssueType.Appearance },
            { "bug", IssueType.Bug },
            { "test", IssueType.Test },
            { "refactor", IssueType.Todo },
            { "refactoring", IssueType.Todo },
        };

        public static List<IssueWrapper> ParseIssues(string input)
        {
            var issues = new List<IssueWrapper>();

            foreach (var line in input.Split(new[] { '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries))
            {
                var tokens = line.Split(',')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();

                string title = null;
                string description = null;
                var estimated = TimeSpan.Zero;
                var issueType = IssueType.Todo; // default fallback

                foreach (var token in tokens)
                {
                    if (TryParseDuration(token, out var duration))
                    {
                        estimated = duration;
                    }
                    else if (IssueTypeMap.TryGetValue(token.ToLowerInvariant(), out var parsedType))
                    {
                        issueType = parsedType;
                    }
                    else
                    {
                        if (title == null)
                        {
                            title = token;
                        }
                        else
                        {
                            description ??= token;
                        }
                    }
                }

                if (title != null)
                {
                    issues.Add(new IssueWrapper
                    {
                        Title = title,
                        Description = description ?? string.Empty,
                        EstimatedDuration = estimated,
                        Type = issueType,
                        State = IssueState.Created,
                        Created = DateTime.Now,
                    });
                }
            }

            return issues;
        }

        private static bool TryParseDuration(string token, out TimeSpan duration)
        {
            duration = TimeSpan.Zero;

            var digits = new string(token.TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(digits, out var minutes))
            {
                duration = TimeSpan.FromMinutes(minutes);
                return true;
            }

            return false;
        }
    }
}