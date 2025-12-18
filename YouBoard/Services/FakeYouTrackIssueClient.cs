using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouBoard.Models;
using YouBoard.Models.Request;

namespace YouBoard.Services
{
    public class FakeYouTrackIssueClient : IYouTrackIssueClient
    {
        public async Task<List<IssueWrapper>> GetIssuesByProjectAsync(string projectShortName, int count = 0, int skip = 0)
        {
            await Task.Delay(1);

            var dummy = new List<IssueWrapper>
            {
                new () { Title = "ダミーIssue 1", Id = "Issue-1", },
                new () { Title = "ダミーIssue 2", Id = "Issue-2", },
                new () { Title = "ダミーIssue 3", Id = "Issue-3", IsExpanded = true, },
                new () { Title = "ダミーIssue 4", Id = "Issue-3", Description = "test Description", },
            };

            dummy[2].Comments.Add(new IssueCommentWrapper() { Text = "Comment1", });
            dummy[2].Comments.Add(new IssueCommentWrapper() { Text = "Comment2", });

            if (skip >= dummy.Count)
            {
                return new List<IssueWrapper>();
            }

            return await Task.FromResult(dummy);
        }

        public Task<List<IssueWrapper>> GetIssuesByProjectAsync(string projectShortName, string titleKeyword, int count = 0, int skip = 0)
        {
            return GetIssuesByProjectAsync(projectShortName, count, skip);
        }

        // New overload: accept IssueSearchOption only and delegate
        public async Task<List<IssueWrapper>> GetIssuesByProjectAsync(IssueSearchOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            var project = option.ProjectShortName;
            var limit = option.Limit;
            var offset = option.Offset;
            var keyword = option.SearchPattern;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await GetIssuesByProjectAsync(project, limit, offset);
            }

            return await GetIssuesByProjectAsync(project, keyword, limit, offset);
        }

        public async Task<IssueWrapper> CreateIssueAsync(string projectShortName, IssueWrapper issueWrapper)
        {
            var dummy = new IssueWrapper()
            {
                Title = "ダミーIssue 1", Id = "Issue-1",
            };

            await Task.Delay(1500);
            return dummy;
        }

        public Task MarkAsCompleteAsync(IssueWrapper issueWrapper)
        {
            return Task.CompletedTask;
        }

        public Task PostIssueStateAsync(IssueWrapper issueWrapper)
        {
            return Task.CompletedTask;
        }

        public Task IssueFieldUpdateRequestAsync(IssueWrapper issueWrapper, IssueUpdatePayload payload)
        {
            Console.WriteLine($"{issueWrapper.Title} (FakeYouTrackIssueClient : 48)");
            Console.WriteLine($"{payload} (FakeYouTrackIssueClient : 48)");
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
            issueWrapper.Comments.Clear();
            issueWrapper.Comments.Add(new IssueCommentWrapper() { Text = "test Comment", });
            return Task.CompletedTask;
        }
    }
}