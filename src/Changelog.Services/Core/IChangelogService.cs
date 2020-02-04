using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeLog.API.Models;

namespace ChangeLog.API.Services.Core
{
    public interface IChangelogService
    {
        Task<IEnumerable<ChangelogData>> GetChangelogEntries();
        Task<ChangelogData> GetChangelog(string version);
        Task<ChangelogData> UpdateChangelog(ChangelogData changeLog);
        Task<bool> RemoveChangelog(ChangelogData changeLog);
    }


    public class StubChangelogService : IChangelogService
    {
        public async Task<ChangelogData> GetChangelog(string version)
        {
            var tasks = await GetChangelogEntries();
            return tasks.FirstOrDefault(x => x.BuildVersion == version);
        }

        public Task<IEnumerable<ChangelogData>> GetChangelogEntries()
        {
            var tasks = new ChangelogData[]
            {
                new ChangelogData()
                {
                    BuildVersion = "1",
                    BuildDate = DateTimeOffset.UtcNow.AddHours(-30),
                    ConcurrencyToken = DateTimeOffset.UtcNow.AddHours(-30),
                    Content = @"List of changes for version 1"
                },
                new ChangelogData()
                {
                    BuildVersion = "2",
                    BuildDate = DateTimeOffset.UtcNow.AddHours(-20),
                    ConcurrencyToken = DateTimeOffset.UtcNow.AddHours(-20),
                    Content = @"Version 2 is super cool"
                },
                new ChangelogData()
                {
                    BuildVersion = "3",
                    BuildDate = DateTimeOffset.UtcNow.AddHours(-10),
                    ConcurrencyToken = DateTimeOffset.UtcNow.AddHours(-10),
                    Content = @"Version 3 is released!"
                }
            };

            return Task.FromResult<IEnumerable<ChangelogData>>(tasks.OrderByDescending(x => x.BuildDate).ToArray());
        }

        public async Task<bool> RemoveChangelog(ChangelogData changeLog)
        {
            var tasks = await GetChangelogEntries();
            return tasks.Any(x => x.BuildVersion == changeLog.BuildVersion);
        }

        public Task<ChangelogData> UpdateChangelog(ChangelogData changeLog)
        {
            return Task.FromResult(changeLog);
        }
    }
}
