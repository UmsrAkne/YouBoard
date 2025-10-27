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
        private const string IssueFieldsQuery =
            "fields=id,idReadable,summary,created,description," +
            "customFields(name,value(name,minutes,presentation))";

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

        public async Task<List<IssueWrapper>> GetIssuesByProjectAsync(string projectShortName, string titleKeyword, int count = 0, int skip = 0)
        {
            if (string.IsNullOrWhiteSpace(projectShortName))
            {
                throw new ArgumentNullException(nameof(projectShortName));
            }

            // キーワード未指定なら既存のオーバーロードへフォールバック
            if (string.IsNullOrWhiteSpace(titleKeyword))
            {
                return await GetIssuesByProjectAsync(projectShortName, count, skip);
            }

            // YouTrackのsummaryフィルタ用に簡易サニタイズ（波括弧は除去）
            var sanitized = titleKeyword.Replace("{", " ").Replace("}", " ").Trim();

            // summary: {キーワード} は部分一致検索。スペースを含む場合にも対応
            var query = $"project:{projectShortName} summary: {{{sanitized}}} sort by: created desc";
            var endpoint = $"issues?query={Uri.EscapeDataString(query)}&{IssueFieldsQuery}";

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
                Created = DateTimeOffset.FromUnixTimeMilliseconds(dto.Created).LocalDateTime,
                IsComplete = dto.IsDone(),
                State = dto.GetState(),
                Type = dto.GetIssueType(),
            }).ToList();
        }

        public async Task<List<IssueWrapper>> GetIssuesByProjectAsync(string projectShortName, int count = 0, int skip = 0)
        {
            var query = $"project:{projectShortName} sort by: created desc";
            var endpoint = $"issues?query={Uri.EscapeDataString(query)}&{IssueFieldsQuery}";

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
                EstimatedDuration = dto.EstimatedDuration,
                Id = dto.IdReadable,
                Title = dto.Summary,
                Created = DateTimeOffset.FromUnixTimeMilliseconds(dto.Created).LocalDateTime,
                IsComplete = dto.IsDone(),
                State = dto.GetState(),
                Type = dto.GetIssueType(),
            }).ToList();
        }

        public async Task<IssueWrapper> CreateIssueAsync(string projectShortName, IssueWrapper issueWrapper)
        {
            var projectId = await GetProjectIdByShortName(projectShortName);

            var customFields = new List<CustomField>
            {
                new()
                {
                    Name = "Type",
                    Value = new FieldValue
                    {
                        Name = IssueTypeHelper.ToIssueTypeName(issueWrapper.Type),
                    },
                    Type = "SingleEnumIssueCustomField",
                },
            };

            // 予測時間を分単位で取得
            var estimatedMinutes = (int)issueWrapper.EstimatedDuration.TotalMinutes;

            var projectFields = await GetProjectFieldNamesAsync(projectId);
            if (estimatedMinutes > 0 && projectFields.Contains("予測"))
            {
                customFields.Add(new CustomField
                {
                    Name = "予測",
                    Value = new FieldValue
                    {
                        ExtensionData = new Dictionary<string, JsonElement>
                        {
                            { "minutes", JsonSerializer.SerializeToElement(estimatedMinutes) },
                        },
                    },
                    Type = "PeriodIssueCustomField",
                });
            }

            var payload = new
            {
                project = new { id = projectId, },
                summary = issueWrapper.Title,
                description = issueWrapper.Description,
                customFields,
            };

            var json = JsonSerializer.Serialize(payload, JsonOptions);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var response = await httpClient.PostAsync($"issues?{IssueFieldsQuery}", content);
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
                Created = DateTimeOffset.FromUnixTimeMilliseconds(created.Created).LocalDateTime,
                Description = created.Description,
                Type = created.GetIssueType(),
                EstimatedDuration = created.EstimatedDuration,
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

        public async Task<IssueCommentWrapper> AddCommentAsync(IssueWrapper issueWrapper, string commentText)
        {
            var payload = new
            {
                text = commentText,
            };

            var json = JsonSerializer.Serialize(payload, JsonOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync(
                $"issues/{issueWrapper.Id}/comments?fields=id,text,author(login,name),created",
                content);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var createdComment = JsonSerializer.Deserialize<YouTrackCommentDto>(responseJson, JsonOptions);

            if (createdComment == null)
            {
                return null;
            }

            return new IssueCommentWrapper
            {
                Id = createdComment.Id,
                Text = createdComment.Text,
                AuthorName = createdComment.Author?.Name,
                Created = createdComment.Created,
            };
        }

        public async Task AddWorkingDurationAsync(IssueWrapper issueWrapper, TimeSpan duration, string comment)
        {
            if (duration < TimeSpan.FromMinutes(1))
            {
                return;
            }

            var payload = new WorkItemAddPayload(duration, comment);
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"issues/{issueWrapper.Id}/timeTracking/workItems?fields=id,text,duration(minutes)";
            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        public async Task LoadCommentsAsync(IssueWrapper issueWrapper)
        {
            var issueId = issueWrapper.Id;
            var url = $"issues/{issueId}/comments?fields=id,text,created";

            using var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, };
            var dtoList = JsonSerializer.Deserialize<List<YouTrackCommentDto>>(json, options);
            var comments = dtoList?.Select(d => new IssueCommentWrapper
            {
                Id = d.Id,
                Text = d.Text,
                Created = d.Created,
            }).ToList();

            if (comments == null)
            {
                return;
            }

            issueWrapper.Comments.Clear();

            foreach (var issueCommentWrapper in comments)
            {
                issueWrapper.Comments.Add(issueCommentWrapper);
            }
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

        private async Task<HashSet<string>> GetProjectFieldNamesAsync(string projectShortName)
        {
            var endpoint = $"admin/projects/{projectShortName}/customFields?fields=id,field(id,name),$type";
            using var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var fields = JsonSerializer.Deserialize<List<JsonElement>>(json, JsonOptions);

            var result = new HashSet<string>();

            if (fields == null)
            {
                return result;
            }

            foreach (var f in fields)
            {
                if (f.TryGetProperty("field", out var fieldObj) &&
                    fieldObj.TryGetProperty("name", out var nameProp))
                {
                    var name = nameProp.GetString();
                    if (!string.IsNullOrEmpty(name))
                    {
                        result.Add(name);
                    }
                }
            }

            return result;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class ProjectDto
        {
            public string ShortName { get; set; }

            public string Id { get; set; }
        }

        private class YouTrackCommentDto
        {
            public string Id { get; init; }

            public string Text { get; init; }

            public AuthorDto Author { get; init; }

            public long Created { get; init; }

            public class AuthorDto
            {
                public string Login { get; set; }

                public string Name { get; set; }
            }
        }
    }
}