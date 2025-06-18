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
            };

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
            throw new System.NotImplementedException();
        }
    }
}