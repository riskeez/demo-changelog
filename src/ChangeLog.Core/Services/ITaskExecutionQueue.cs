using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeLog.Core.Services
{
    public interface ITaskExecutionPool
    {
        Task<TaskData> GetNextTaskAsync(CancellationToken token);

        IEnumerable<TaskData> GetActiveTasksAsync();

        Task<ActiveTaskData> SetActiveAsync(TaskData task, CancellationToken token = default);

        Task<TaskData> SetCancelledAsync(int taskId, CancellationToken token = default);

        Task<TaskData> SetFinishedAsync(int taskId, CancellationToken token = default);

        Task SetTaskProgressAsync(int taskId, int reportValue, string message = null, CancellationToken token = default);
    }
}
