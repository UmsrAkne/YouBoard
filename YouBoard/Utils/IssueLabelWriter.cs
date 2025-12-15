using System;
using YouBoard.Models;

namespace YouBoard.Utils
{
    public static class IssueLabelWriter
    {
        // ReSharper disable once ArrangeModifiersOrder
        private static readonly string[] SpinnerFrames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏", };

        private static int spinnerIndex;

        public static string GenerateWindowTitle(IssueWrapper issueWrapper, string spinnerFrame = null, TimeSpan? elapsedTimeSpan = null)
        {
            if (issueWrapper.State != IssueState.Working)
            {
                return string.Empty;
            }

            var totalMin = elapsedTimeSpan == null
                ? (int)issueWrapper.WorkTimer.Elapsed.TotalMinutes
                : (int)elapsedTimeSpan.Value.TotalMinutes;

            var frame = string.IsNullOrWhiteSpace(spinnerFrame)
                ? SpinnerFrames[spinnerIndex++ % SpinnerFrames.Length]
                : spinnerFrame;

            var entryNo = issueWrapper.EntryNo;
            return $"[{totalMin}m {frame}] {issueWrapper.Title}" + (entryNo != 0 ? $" #{entryNo}" : string.Empty);
        }

        public static string GenerateStatusLabel(IssueWrapper issueWrapper)
        {
            if (issueWrapper.State != IssueState.Working)
            {
                return issueWrapper.State.ToString();
            }

            const string format = "hh':'mm':'ss";

            var elapsedStr = issueWrapper.WorkTimer.Elapsed.ToString(format);

            if (issueWrapper.ElapsedDuration != TimeSpan.Zero)
            {
                elapsedStr += $"  +{(int)issueWrapper.ElapsedDuration.TotalMinutes}min";
            }

            return elapsedStr;
        }
    }
}