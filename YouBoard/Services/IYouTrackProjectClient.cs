using System.Collections.Generic;
using System.Threading.Tasks;
using YouBoard.Models;

namespace YouBoard.Services
{
    public interface IYouTrackProjectClient
    {
        Task<List<ProjectWrapper>> GetProjectsAsync();

        Task<List<ProjectWrapper>> MergeProjectsWithRemoteData(List<ProjectWrapper> localProjects);

        List<ProjectWrapper> LoadProjectsFromJsonFile(string fileName);

        public void SaveProjectsToJsonFile(List<ProjectWrapper> projects, string fileName);
    }
}