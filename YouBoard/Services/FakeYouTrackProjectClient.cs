using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using YouBoard.Models;

namespace YouBoard.Services
{
    public class FakeYouTrackProjectClient : IYouTrackProjectClient
    {
        public Task<List<ProjectWrapper>> GetProjectsAsync()
        {
            var dummy = new List<ProjectWrapper>();

            for (var i = 0; i < 15; i++)
            {
                dummy.Add(new ProjectWrapper() { Name = $"ダミープロジェクト{i}", ShortName = $"DM{i}", });
            }

            return Task.FromResult(dummy);
        }

        public async Task<List<ProjectWrapper>> MergeProjectsWithRemoteData(List<ProjectWrapper> localProjects)
        {
            var r = await GetProjectsAsync();
            return r;
        }

        public List<ProjectWrapper> LoadProjectsFromJsonFile(string fileName)
        {
            // Debug-friendly implementation: load a list from JSON if available, otherwise return an empty list
            var empty = new List<ProjectWrapper>();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return empty;
            }

            try
            {
                var path = ResolvePath(fileName);
                if (!File.Exists(path))
                {
                    return empty;
                }

                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return empty;
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, };
                return JsonSerializer.Deserialize<List<ProjectWrapper>>(json, options) ?? empty;
            }
            catch
            {
                // For debug usage, fail gracefully and just return empty
                return empty;
            }
        }

        public void SaveProjectsToJsonFile(List<ProjectWrapper> projects, string fileName)
        {
            // Debug-friendly implementation: write JSON to the specified file, creating directories when needed
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            try
            {
                var path = ResolvePath(fileName);
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var options = new JsonSerializerOptions { WriteIndented = true, };
                var json = JsonSerializer.Serialize(projects ?? new List<ProjectWrapper>(), options);
                File.WriteAllText(path, json);
            }
            catch
            {
                // Swallow errors in a fake client to avoid breaking debug flows
            }
        }

        private static string ResolvePath(string fileName)
        {
            // If fileName is rooted, use as-is. Otherwise, resolve relative to the current working directory.
            return Path.IsPathRooted(fileName) ? fileName : Path.Combine(Directory.GetCurrentDirectory(), fileName);
        }
    }
}