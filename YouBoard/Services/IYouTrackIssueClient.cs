using System.Collections.Generic;
using System.Threading.Tasks;
using YouBoard.Models;

namespace YouBoard.Services
{
    public interface IYouTrackIssueClient
    {
        Task<List<IssueWrapper>> GetIssuesByProjectAsync(string projectShortName, int count = 0, int skip = 0);

        Task<IssueWrapper> CreateIssueAsync(string projectShortName, IssueWrapper issueWrapper);
    }
}