using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using YouBoard.Models;

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

        public async Task<List<IssueWrapper>> GetIssuesByProjectAsync(string projectShortName, int count = 0)
        {
            var endpoint = $"issues?query=project:{projectShortName}&fields=id,idReadable,summary";
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
            }).ToList();
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

        // ReSharper disable once ClassNeverInstantiated.Local
        private class YouTrackIssueDto
        {
            public string IdReadable { get; set; } = string.Empty;

            public string Summary { get; set; } = string.Empty;
        }
    }
}