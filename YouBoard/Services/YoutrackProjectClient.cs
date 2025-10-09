using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using YouBoard.Models;

namespace YouBoard.Services
{
    public class YoutrackProjectClient : IYouTrackProjectClient, IDisposable
    {
        // ReSharper disable once ArrangeModifiersOrder
        private static readonly JsonSerializerOptions JsonOptions = new ()
        {
            PropertyNameCaseInsensitive = true,
        };

        private readonly HttpClient httpClient;
        private readonly string baseUrl;
        private readonly string token;

        public YoutrackProjectClient(string baseUrl, string token)
        {
            this.baseUrl = baseUrl.TrimEnd('/');
            this.token = token ?? throw new ArgumentNullException(nameof(token));

            httpClient = new HttpClient
            {
                BaseAddress = new Uri($"{this.baseUrl}/api/"),
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static void SaveProjectsToJsonFile(List<ProjectWrapper> projects, string fileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true, };
            var json = JsonSerializer.Serialize(projects, options);
            var d = Directory.CreateDirectory("local_data");
            File.WriteAllText(Path.Combine(d.FullName, fileName), json);
        }

        public static List<ProjectWrapper> LoadProjectsFromJsonFile(string fileName)
        {
            var d = Directory.CreateDirectory("local_data");
            var path = Path.Combine(d.FullName, fileName);
            if (!File.Exists(path))
            {
                SaveProjectsToJsonFile(new List<ProjectWrapper>(), fileName);
            }

            var json = File.ReadAllText(path);
            var projects = JsonSerializer.Deserialize<List<ProjectWrapper>>(json);

            return projects ?? new List<ProjectWrapper>();
        }

        public async Task<List<ProjectWrapper>> GetProjectsAsync()
        {
            var result = new List<ProjectWrapper>();
            const int pageSize = 200;
            var skip = 0;

            while (true)
            {
                var endpoint = $"admin/projects?fields=id,name,shortName&$skip={skip}&$top={pageSize}";

                using var response = await httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var batch = JsonSerializer.Deserialize<List<YouTrackProjectDto>>(json, JsonOptions) 
                            ?? new List<YouTrackProjectDto>();

                if (batch.Count == 0)
                {
                    break;
                }

                result.AddRange(batch.Select(dto => new ProjectWrapper
                {
                    Name = dto.Name,
                    ShortName = dto.ShortName,
                }));

                if (batch.Count < pageSize)
                {
                    // 最終ページ
                    break;
                }

                skip += batch.Count;
            }

            return result;
        }

        public async Task<List<ProjectWrapper>> MergeProjectsWithRemoteData(List<ProjectWrapper> localProjects)
        {
            var remotes = await GetProjectsAsync();

            // 取得失敗や認可不足等で 0 件だった場合はローカルを保持（安全策）
            if (remotes.Count == 0)
            {
                return localProjects.OrderBy(p => p.ShortName).ToList();
            }

            // 万一の重複 shortName を許容して先勝ちにする
            var remotesDic = remotes
                .GroupBy(p => p.ShortName)
                .ToDictionary(g => g.Key, g => g.First());

            var toRemove = new List<ProjectWrapper>();

            foreach (var projectWrapper in localProjects)
            {
                if (remotesDic.TryGetValue(projectWrapper.ShortName, out var _))
                {
                    remotesDic.Remove(projectWrapper.ShortName);
                }
                else
                {
                    toRemove.Add(projectWrapper);
                }
            }

            foreach (var projectWrapper in toRemove)
            {
                localProjects.Remove(projectWrapper);
            }

            if (remotesDic.Count > 0)
            {
                localProjects.AddRange(remotesDic.Select(kv => kv.Value));
            }

            return localProjects.OrderBy(p => p.ShortName).ToList();
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
        private class YouTrackProjectDto
        {
            public string Name { get; set; } = string.Empty;

            public string ShortName { get; set; } = string.Empty;
        }
    }
}