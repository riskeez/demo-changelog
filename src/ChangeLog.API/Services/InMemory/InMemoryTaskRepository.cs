using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ChangeLog.Core.Services;
using ChangeLog.Core;
using System.Linq.Expressions;

namespace ChangeLog.API.Services
{
    public class InMemoryTaskRepository : ITaskRepository
    {
        private readonly ConcurrentDictionary<int, TaskData> _tasks = new ConcurrentDictionary<int, TaskData>();

        private readonly ILogger _logger;
        private int _taskCounter = 0;

        public InMemoryTaskRepository(ILogger<InMemoryTaskRepository> logger)
        {
            _logger = logger;
        }

        protected int GetNewTaskId() => Interlocked.Increment(ref _taskCounter);

        public Task<TaskData> AddAsync(TaskData task, CancellationToken token = default)
        {
            task.Id = GetNewTaskId();

            if (_tasks.TryAdd(task.Id, task))
            {
                _logger.LogInformation($"Task {task.Id} is added to repository");

                return Task.FromResult(task);
            }

            _logger.LogError($"Task already exists. Data: {JsonConvert.SerializeObject(task)}");

            throw new ArgumentException($"Task {task.Id} already exixst");
        }

        public Task<TaskData> UpdateAsync(TaskData task, CancellationToken token = default)
        {
            if (_tasks.TryGetValue(task.Id, out TaskData oldTask))
            {
                return Task.FromResult(oldTask);
            }

            _logger.LogWarning($"Task {task.Id} does not exist");
            return null;
        }

        public Task<TaskData> GetAsync(int id, CancellationToken token = default)
        {
            if (_tasks.TryGetValue(id, out TaskData task))
            {
                return Task.FromResult(task);
            }

            _logger.LogWarning($"Task {id} not found");
            return null;
        }

        public Task<IEnumerable<TaskData>> GetTasksAsync(Expression<Func<TaskData, bool>> predicate, int count = -1, CancellationToken token = default)
        {
            var tasks = _tasks.Values.AsEnumerable();
            if (predicate != null)
            {
                tasks = tasks.Where(predicate.Compile()).OrderBy(x => x.CreatedAt);
            }
            if (count > -1) 
            {
                tasks.Take(count);
            }
            
            return Task.FromResult<IEnumerable<TaskData>>(tasks.ToArray());
        }

        public Task RemoveAsync(int id, CancellationToken token = default)
        {
            if (_tasks.TryRemove(id, out TaskData removedTask))
            {
                _logger.LogInformation($"Task {id} is removed. Data: {JsonConvert.SerializeObject(removedTask)}");
            }
            else
            {
                _logger.LogWarning($"Task {id} does not exist");
            }

            return Task.CompletedTask;
        }
    }
}
