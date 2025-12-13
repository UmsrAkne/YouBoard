using YouBoard.Models;
using YouBoard.Utils;

namespace YouBoard.Test.Utils;

public class IssueLabelWriterTests
{
    [Test]
    public void GenerateWindowTitle_ReturnsEmpty_WhenStateNotWorking()
    {
        var issue = new IssueWrapper
        {
            Title = "Some Task",
            State = IssueState.Created,
            EntryNo = 0,
        };

        var title = IssueLabelWriter.GenerateWindowTitle(issue, "*", TimeSpan.FromMinutes(5));
        Assert.That(title, Is.EqualTo(string.Empty));
    }

    [Test]
    public void GenerateWindowTitle_FormatsCorrectly_NoEntryNo()
    {
        var issue = new IssueWrapper
        {
            Title = "Task A",
            State = IssueState.Working,
            EntryNo = 0,
        };

        var title = IssueLabelWriter.GenerateWindowTitle(issue, "*", TimeSpan.Zero);
        Assert.That(title, Is.EqualTo("[0m *] Task A"));
    }

    [Test]
    public void GenerateWindowTitle_FormatsCorrectly_WithEntryNo()
    {
        var issue = new IssueWrapper
        {
            Title = "Task B",
            State = IssueState.Working,
            EntryNo = 3,
        };

        var title = IssueLabelWriter.GenerateWindowTitle(issue, ">", TimeSpan.FromMinutes(12));
        Assert.That(title, Is.EqualTo("[12m >] Task B #3"));
    }

    [Test]
    public void GenerateWindowTitle_UsesFloorOfElapsedMinutes()
    {
        var issue = new IssueWrapper
        {
            Title = "Task C",
            State = IssueState.Working,
            EntryNo = 1,
        };

        // 12 minutes and 59 seconds -> cast to int should floor to 12
        var ts = TimeSpan.FromMinutes(12) + TimeSpan.FromSeconds(59);

        var title = IssueLabelWriter.GenerateWindowTitle(issue, "~", ts);
        Assert.That(title, Is.EqualTo("[12m ~] Task C #1"));
    }
}