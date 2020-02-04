using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ChangeLog.Core.Services;
using ChangeLog.Core;

namespace ChangeLog.API.Services
{
    public class TaskExecutionPool : ITaskExecutionPool, IDisposable
    {
        private readonly ITaskRepository _taskRepo;
        private readonly ILogger _logger;

        private ConcurrentDictionary<int, ActiveTaskData> _activeTasks = new ConcurrentDictionary<int, ActiveTaskData>();

        public TaskExecutionPool(ITaskRepository taskRepository, ILogger<TaskExecutionPool> logger)
        {
            _taskRepo = taskRepository ?? throw new ArgumentNullException(nameof(ITaskRepository));
            _logger = logger;
        }

        public async Task<TaskData> GetNextTaskAsync(CancellationToken token = default)
        {
            var tasks = await _taskRepo.GetTasksAsync(x => x.Status == TaskState.Planned, 1, token);

            return tasks.FirstOrDefault();
        }

        public async Task<ActiveTaskData> SetActiveAsync(TaskData task, CancellationToken token = default)
        {
            ActiveTaskData activeTask = new ActiveTaskData();

            if (_activeTasks.TryAdd(task.Id, activeTask))
            {
                task.Status = TaskState.Active;
                task.StartedAt = DateTimeOffset.UtcNow;

                activeTask.Init(task);

                _logger.LogInformation($"Task {task.Id} is activated");

                await _taskRepo.UpdateAsync(task, token);

                return activeTask;
            }

            return null;
        }

        public async Task<TaskData> SetCancelledAsync(int taskId, CancellationToken token = default)
        {
            if (_activeTasks.TryRemove(taskId, out ActiveTaskData runningTask))
            {
                runningTask.CancellationTokenSource.Cancel();

                runningTask.Data.Status = TaskState.Cancelled;
                runningTask.Data.FinishedAt = DateTimeOffset.UtcNow;

                _logger.LogInformation($"Task {taskId} is cancelled");

                await _taskRepo.UpdateAsync(runningTask.Data, token);

                runningTask.Dispose();
            }

            return null;
        }

        public async Task<TaskData> SetFinishedAsync(int taskId, CancellationToken token = default)
        {
            if (_activeTasks.TryRemove(taskId, out ActiveTaskData finishedTask))
            {
                finishedTask.Data.Status = TaskState.Finished;
                finishedTask.Data.FinishedAt = DateTimeOffset.UtcNow;

                _logger.LogInformation($"Task {taskId} is finished");

                await _taskRepo.UpdateAsync(finishedTask.Data, token);

                finishedTask.Dispose();
            }

            return null;
        }

        public async Task SetTaskProgressAsync(int taskId, int value, string message = null, CancellationToken token = default)
        {
            if (_activeTasks.TryGetValue(taskId, out ActiveTaskData activeTask))
            {
                activeTask.Data.PercentComplete = value;

                await _taskRepo.UpdateAsync(activeTask.Data, token);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposing) return;

            if (_activeTasks != null)
            {
                foreach (ActiveTaskData activeTask in _activeTasks.Values)
                {
                    activeTask.Dispose();
                }
                _activeTasks = null;
            }
        }
    }
}
