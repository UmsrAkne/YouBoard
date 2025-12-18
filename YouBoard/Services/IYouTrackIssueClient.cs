using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouBoard.Models;
using YouBoard.Models.Request;

namespace YouBoard.Services
{
    public interface IYouTrackIssueClient
    {
        Task<List<IssueWrapper>> GetIssuesByProjectAsync(string projectShortName, int count = 0, int skip = 0);

        Task<List<IssueWrapper>> GetIssuesByProjectAsync(string projectShortName, string titleKeyword, int count = 0, int skip = 0);

        // New overload: accept IssueSearchOption only
        Task<List<IssueWrapper>> GetIssuesByProjectAsync(IssueSearchOption option);

        Task<IssueWrapper> CreateIssueAsync(string projectShortName, IssueWrapper issueWrapper);

        Task MarkAsCompleteAsync(IssueWrapper issueWrapper);

        Task PostIssueStateAsync(IssueWrapper issueWrapper);

        Task IssueFieldUpdateRequestAsync(IssueWrapper issueWrapper, IssueUpdatePayload payload);

        Task<IssueCommentWrapper> AddCommentAsync(IssueWrapper issueWrapper, string comment);

        Task AddWorkingDurationAsync(IssueWrapper issueWrapper, TimeSpan duration, string comment);

        Task LoadCommentsAsync(IssueWrapper issueWrapper);
    }
}