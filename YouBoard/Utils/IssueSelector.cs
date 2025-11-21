using System.Collections.Generic;
using System.Linq;
using YouBoard.Models;

namespace YouBoard.Utils
{
    public class IssueSelector
    {
        /// <summary>
        /// Returns the maximum EntryNo among issues whose title matches the base issue's title.
        /// Title comparison is performed after trimming leading and trailing whitespace.
        /// If there is no match or the sequence is empty, it returns 0.
        /// </summary>
        /// <param name="issues">A sequence of issues to search.</param>
        /// <param name="baseIssue">The reference issue whose Title is used for matching.</param>
        /// <returns>
        /// The maximum EntryNo among issues with the same trimmed Title as <paramref name="baseIssue"/>;
        /// returns 0 when no matching issues are found or the sequence is empty.
        /// </returns>
        public static int GetMaxEntryNoForSameTitle(IEnumerable<IssueWrapper> issues, IssueWrapper baseIssue)
        {
            var issueWrappers = issues.ToList();
            if (!issueWrappers.Any())
            {
                return 0;
            }

            var matchedTitleIssues = issueWrappers
                .Where(w => baseIssue.Title.Trim() == w.Title.Trim())
                .OrderByDescending(w => w.EntryNo).ToList();

            if (matchedTitleIssues.Any())
            {
                return matchedTitleIssues.First().EntryNo;
            }

            return 0;
        }
    }
}