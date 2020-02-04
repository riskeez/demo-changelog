using ChangeLog.API.Data;
using ChangeLog.Core.Services;
using ChangeLog.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeLog.API.Services
{
    public class EFTaskRepository : ITaskRepository
    {
        private readonly TaskDbContext _dbContext;
        private readonly ILogger _logger;

        public EFTaskRepository(TaskDbContext dbContext, ILogger<EFTaskRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger;
        }

        public async Task<TaskData> AddAsync(TaskData task, CancellationToken token = default)
        {
            var data = await _dbContext.Tasks.AddAsync(task, token);
            _logger.LogInformation($"Task {task.Id} added");

            await _dbContext.SaveChangesAsync(token);

            return data.Entity;
        }

        public Task<TaskData> GetAsync(int id, CancellationToken token = default)
        {
            return _dbContext.Tasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<TaskData>> GetTasksAsync(Expression<Func<TaskData, bool>> predicate, int count = -1, CancellationToken token = default)
        {
            var query = _dbContext.Tasks.AsNoTracking();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (count > -1)
            {
                query = query.Take(count);
            }

            return await query.ToArrayAsync(token);
        }

        public async Task RemoveAsync(int id, CancellationToken token = default)
        {
            TaskData task = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (task != null)
            {
                _dbContext.Tasks.Remove(task);
                await _dbContext.SaveChangesAsync(token);
            }
        }

        public async Task<TaskData> UpdateAsync(TaskData task, CancellationToken token = default)
        {
            TaskData dbtask = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == task.Id);
            _dbContext.Entry(dbtask).CurrentValues.SetValues(task);

            await _dbContext.SaveChangesAsync();
            return dbtask;
        }
    }
}
