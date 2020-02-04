using ChangeLog.API.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeLog.API.Services.Core
{
    public interface ITaskExecutionPool
    {
        Task<TaskData> GetNextTaskAsync(CancellationToken token);

        Task<ActiveTaskData> SetActiveAsync(TaskData task, CancellationToken token = default);

        Task<TaskData> SetCancelledAsync(int taskId, CancellationToken token = default);

        Task<TaskData> SetFinishedAsync(int taskId, CancellationToken token = default);

        Task SetTaskProgressAsync(int taskId, int reportValue, string message = null, CancellationToken token = default);
    }
}
