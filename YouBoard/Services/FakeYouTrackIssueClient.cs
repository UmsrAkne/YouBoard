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
    }
}