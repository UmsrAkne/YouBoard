using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YouBoard.Models;
using YouBoard.Models.Request;
using YouBoard.Utils;

namespace YouBoard.Services
{
    public class YouTrackIssueClient : IYouTrackIssueClient, IDisposable
    {
        // ReSharper disable once ArrangeModifiersOrder
        private static readonly JsonSerializerOptions JsonOptions = new ()
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly HttpClient httpClient;

        public YouTrackIssueClient(string baseUrl, string token)
        {
            var url = baseUrl.TrimEnd('/');
            var t = token ?? throw new ArgumentNullException(nameof(token));

            httpClient = new HttpClient
            {
                BaseAddress = new Uri($"{url}/api/"),
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", t);
        }

        public async Task<List<IssueWrapper>> GetIssuesByProjectAsync(string projectShortName, int count = 0, int skip = 0)
        {
            var query = $"project:{projectShortName} sort by: created desc";
            var endpoint = $"issues?query={Uri.EscapeDataString(query)}&fields=id,idReadable,summary,customFields(name,value(name))";

            if (skip != 0)
            {
                endpoint += $"&$skip={skip}";
            }

            if (count != 0)
            {
                endpoint += $"&$top={count}";
            }

            using var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var rawIssues = JsonSerializer.Deserialize<List<YouTrackIssueDto>>(json, JsonOptions);
            if (rawIssues == null)
            {
                return new List<IssueWrapper>();
            }

            return rawIssues.Select(dto => new IssueWrapper
            {
                Id = dto.IdReadable,
                Title = dto.Summary,
                IsComplete = dto.IsDone(),
                State = dto.GetState(),
            }).ToList();
        }

        public async Task<IssueWrapper> CreateIssueAsync(string projectShortName, IssueWrapper issueWrapper)
        {
            var projectId = await GetProjectIdByShortName(projectShortName);
            var payload = new
            {
                project = new { id = projectId, },
                summary = issueWrapper.Title,
                description = issueWrapper.Description,
            };

            var json = JsonSerializer.Serialize(payload, JsonOptions);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var response = await httpClient.PostAsync("issues?fields=id,idReadable,summary,description", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var created = JsonSerializer.Deserialize<YouTrackIssueDto>(responseJson, JsonOptions);

            if (created == null)
            {
                return null;
            }

            return new IssueWrapper
            {
                Id = created.IdReadable,
                Title = created.Summary,
                Description = created.Description,
            };
        }

        public async Task MarkAsCompleteAsync(IssueWrapper issueWrapper)
        {
            var payload = new IssueStateChangePayload(IssueState.Complete.ToIssueStateName());
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"issues/{issueWrapper.Id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task ToggleIssueWorkStateAsync(IssueWrapper issueWrapper)
        {
            var destState = issueWrapper.State switch
            {
                IssueState.Created => IssueState.Working,
                IssueState.Paused => IssueState.Working,
                IssueState.Working => IssueState.Created,
                _ => throw new ArgumentOutOfRangeException(),
            };

            var payload = new IssueStateChangePayload(destState.ToIssueStateName());
            issueWrapper.State = destState;

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"issues/{issueWrapper.Id}", content);
            response.EnsureSuccessStatusCode();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            httpClient?.Dispose();
        }

        private async Task<string> GetProjectIdByShortName(string shortName)
        {
            var endpoint = $"admin/projects?query={shortName}&fields=id,shortName";
            var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var projects = JsonSerializer.Deserialize<List<ProjectDto>>(json, JsonOptions);
            return projects?.FirstOrDefault(p => p.ShortName == shortName)?.Id;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class ProjectDto
        {
            public string ShortName { get; set; }

            public string Id { get; set; }
        }
    }
}