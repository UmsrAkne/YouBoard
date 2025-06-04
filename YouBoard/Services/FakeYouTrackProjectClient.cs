using System.Collections.Generic;
using System.Threading.Tasks;
using YouBoard.Models;

namespace YouBoard.Services
{
    public class FakeYouTrackProjectClient : IYouTrackProjectClient
    {
        public Task<List<ProjectWrapper>> GetProjectsAsync()
        {
            var dummy = new List<ProjectWrapper>
            {
                new () { Name = "ダミープロジェクト1", ShortName = "DM1", },
                new () { Name = "ダミープロジェクト2", ShortName = "DM2", },
            };

            return Task.FromResult(dummy);
        }
    }
}