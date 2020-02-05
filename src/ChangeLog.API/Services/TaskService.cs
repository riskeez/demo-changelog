using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using ChangeLog.Core.Services;
using ChangeLog.Core;

namespace ChangeLog.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskRunningService _taskRunner;
        private readonly ILogger _logger;

        public TaskService(ITaskRepository taskRepo, ITaskRunningService taskRunner, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepo ?? throw new ArgumentNullException(nameof(ITaskRepository));
            _taskRunner = taskRunner ?? throw new ArgumentNullException(nameof(ITaskRunningService));
            _logger = logger;
        }

        public async Task<TaskData> AddTask(string createdBy, TaskPayload payload)
        {
            try
            {
                var newTask = new TaskData()
                {
                    CreatedBy = createdBy,
                    Payload = payload,
                };

                var result = await _taskRepository.AddAsync(newTask);

                _= _taskRunner.TryRunTask();
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Add task by: {createdBy}. Data: {JsonConvert.SerializeObject(payload)}");
            }

            return null;
        }

        public Task<TaskData> GetTask(int taskId)
        {
            try
            {
                return _taskRepository.GetAsync(taskId);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Get task {taskId}");
            }

            return null;
        }

        public Task<IEnumerable<TaskData>> GetTasks(bool activeOnly, int count)
        {
            if (activeOnly)
            {
                return _taskRepository.GetAsync(x => x.Status == TaskState.Active, count);
            }
            return _taskRepository.GetAsync(null, count);
        }

        public async Task<bool> RemoveTask(int taskId)
        {
            bool isSucceeded = false;
            try
            {
                await _taskRepository.RemoveAsync(taskId);

                _ = _taskRunner.TryRunTask();

                isSucceeded = true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Delete task {taskId}");
            }

            return isSucceeded;
        }

        public async Task<bool> CancelTask(int taskId)
        {
            try
            {
                var cancelResult = await _taskRunner.TryStopTask(taskId);

                return cancelResult.Result == ExecuteResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Cancel task {taskId}");
            }
            return false;
        }
    }
}
