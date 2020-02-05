using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ChangeLog.Core.Services;
using ChangeLog.Core;

namespace ChangeLog.API.Services
{
    public class TaskRunningService : ITaskRunningService
    {
        private readonly ITaskExecutionPool _taskPool;
        private readonly ILogger _logger;

        public TaskRunningService(ITaskExecutionPool pool, ILogger<TaskRunningService> logger)
        {
            _taskPool = pool ?? throw new ArgumentNullException(nameof(ITaskExecutionPool));
            _logger = logger;
        }

        /// <summary>
        /// Try to stop active task
        /// </summary>
        /// <param name="activeTaskId"></param>
        /// <returns></returns>
        public async Task<ExecutionResult> TryStopTask(int activeTaskId)
        {
            ExecutionResult execResult = new ExecutionResult() { TaskId = activeTaskId, Result = ExecuteResult.NoResult };
            try
            {
                TaskData cancelledTask = await _taskPool.SetCancelledAsync(activeTaskId);
                if (cancelledTask != null)
                {
                    execResult.Result = ExecuteResult.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Task {activeTaskId} stop execution error");

                execResult.Result = ExecuteResult.Error;
            }
            return execResult;
        }

        /// <summary>
        /// Try to run a next task in the task queue
        /// </summary>
        /// <returns></returns>
        public async Task<ExecutionResult> TryRunTask()
        {
            CancellationToken cancellationToken = default;

            ExecutionResult execResult = new ExecutionResult() { Result = ExecuteResult.NoResult };
            try
            {
                TaskData task = await _taskPool.GetNextTaskAsync(cancellationToken);
                if (task == null)
                {
                    return execResult;
                }

                var activeTask = await _taskPool.SetActiveAsync(task, cancellationToken);
                if (activeTask == null)
                {
                    _logger.LogTrace("Task activation failed");
                    return execResult;
                }

                task = activeTask.Data;

                execResult.TaskId = task.Id;
                execResult.Result = await LongRunningProcess(task, activeTask.Token, _taskPool.SetTaskProgressAsync);

                switch (execResult.Result)
                {
                    case ExecuteResult.Success:
                        task = await _taskPool.SetFinishedAsync(task.Id, cancellationToken);
                        break;
                    case ExecuteResult.Cancelled:
                        task = await _taskPool.SetCancelledAsync(task.Id);
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Cancel request");

                execResult.Result = ExecuteResult.Cancelled;
                return execResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Task execution exception");

                execResult.Result = ExecuteResult.Error;
            }

            return execResult;
        }

        public delegate Task ProgressCallbackAsync(int taskId, int value, string message = null, CancellationToken token = default);

        private async Task<ExecuteResult> LongRunningProcess(TaskData task, CancellationToken token, ProgressCallbackAsync progressReportAsync)
        {
            var rand = new Random(DateTimeOffset.UtcNow.Millisecond);

            await Task.Delay(rand.Next(3000, 5000));

            token.ThrowIfCancellationRequested();

            await progressReportAsync(task.Id, 50, "Half done!", token);

            token.ThrowIfCancellationRequested();
            await Task.Delay(rand.Next(5000, 7000));

            token.ThrowIfCancellationRequested();
            return ExecuteResult.Success;
        }
    }
}
