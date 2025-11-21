using YouBoard.Models;
using YouBoard.Utils;

namespace YouBoard.Test.Utils
{
    public class IssueSelectorTests
    {
        [Test]
        public void GetMaxEntryNo_EmptyList_ReturnsZero()
        {
            // arrange
            var issues = Array.Empty<IssueWrapper>();
            var baseIssue = new IssueWrapper { Title = "Any", };

            // act
            var result = IssueSelector.GetMaxEntryNoForSameTitle(issues, baseIssue);

            // assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetMaxEntryNo_NoMatchingTitles_ReturnsZero()
        {
            // arrange
            var issues = new[]
            {
                new IssueWrapper { Title = "Foo", EntryNo = 1, },
                new IssueWrapper { Title = "Bar", EntryNo = 3, },
            };

            var baseIssue = new IssueWrapper { Title = "Baz", };

            // act
            var result = IssueSelector.GetMaxEntryNoForSameTitle(issues, baseIssue);

            // assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetMaxEntryNo_MatchingTitles_ReturnsMaxEntryNo()
        {
            // arrange
            var issues = new[]
            {
                new IssueWrapper { Title = "Task A", EntryNo = 2, },
                new IssueWrapper { Title = "Task A", EntryNo = 5, },
                new IssueWrapper { Title = "Task B", EntryNo = 7, },
            };

            var baseIssue = new IssueWrapper { Title = "Task A", };

            // act
            var result = IssueSelector.GetMaxEntryNoForSameTitle(issues, baseIssue);

            // assert
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void GetMaxEntryNo_TitleComparison_TrimsWhitespace()
        {
            // arrange
            var issues = new[]
            {
                new IssueWrapper { Title = "  Hello World  ", EntryNo = 10, },
                new IssueWrapper { Title = "Hello World", EntryNo = 12, },
            };

            var baseIssue = new IssueWrapper { Title = "Hello World   ", };

            // act
            var result = IssueSelector.GetMaxEntryNoForSameTitle(issues, baseIssue);

            // assert
            Assert.That(result, Is.EqualTo(12));
        }
    }
}