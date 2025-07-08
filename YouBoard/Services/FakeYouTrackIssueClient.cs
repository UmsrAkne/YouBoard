using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouBoard.Models;

namespace YouBoard.Services
{
    public class FakeYouTrackIssueClient : IYouTrackIssueClient
    {
        public Task<List<IssueWrapper>> GetIssuesByProjectAsync(string projectShortName, int count = 0, int skip = 0)
        {
            var dummy = new List<IssueWrapper>
            {
                new () { Title = "ダミーIssue 1", Id = "Issue-1", },
                new () { Title = "ダミーIssue 2", Id = "Issue-2", },
                new () { Title = "ダミーIssue 3", Id = "Issue-3", IsExpanded = true, },
                new () { Title = "ダミーIssue 4", Id = "Issue-3", Description = "test Description", },
            };

            dummy[2].Comments.Add(new IssueCommentWrapper() { Text = "Comment1", });
            dummy[2].Comments.Add(new IssueCommentWrapper() { Text = "Comment2", });

            return Task.FromResult(dummy);
        }

        public Task<IssueWrapper> CreateIssueAsync(string projectShortName, IssueWrapper issueWrapper)
        {
            var dummy = new IssueWrapper()
            {
                Title = "ダミーIssue 1", Id = "Issue-1",
            };

            return Task.FromResult(dummy);
        }

        public Task MarkAsCompleteAsync(IssueWrapper issueWrapper)
        {
            return Task.CompletedTask;
        }

        public Task ToggleIssueWorkStateAsync(IssueWrapper issueWrapper)
        {
            if (issueWrapper.State == IssueState.Created)
            {
                issueWrapper.State = IssueState.Working;
            }
            else if (issueWrapper.State == IssueState.Working)
            {
                issueWrapper.State = IssueState.Created;
            }

            return Task.CompletedTask;
        }

        public Task<IssueCommentWrapper> AddCommentAsync(IssueWrapper issueWrapper, string comment)
        {
            var commentWrapper = new IssueCommentWrapper() { Text = comment, };
            issueWrapper.Comments.Add(commentWrapper);
            return Task.FromResult(commentWrapper);
        }

        public Task AddWorkingDurationAsync(IssueWrapper issueWrapper, TimeSpan duration, string comment)
        {
            Console.WriteLine($"{issueWrapper.Title} += {duration}(FakeYouTrackIssueClient : 58)");
            return Task.CompletedTask;
        }

        public Task LoadCommentsAsync(IssueWrapper issueWrapper)
        {
            issueWrapper.Comments.Add(new IssueCommentWrapper() { Text = "test Comment", });
            return Task.CompletedTask;
        }
    }
}