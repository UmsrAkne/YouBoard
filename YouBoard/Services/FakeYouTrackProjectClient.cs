using System.Collections.Generic;
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

        public Task<List<ProjectWrapper>> MergeProjectsWithRemoteData(List<ProjectWrapper> localProjects)
        {
            return Task.FromResult(localProjects);
        }
    }
}