using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeLog.Core.Services
{
    public interface IChangelogService
    {
        Task<IEnumerable<ChangelogData>> GetChangelogEntries();
        Task<ChangelogData> GetChangelog(string version);
        Task<ChangelogData> UpdateChangelog(ChangelogData changeLog);
        Task<bool> RemoveChangelog(ChangelogData changeLog);
    }
}
