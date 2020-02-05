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

namespace ChangeLog.Domain
{
    public class EFTaskRepository : ITaskRepository
    {
        private readonly DbContextOptions<TaskDbContext> _dbOptions;
        private readonly ILogger _logger;

        public EFTaskRepository(DbContextOptions<TaskDbContext> dbOptions, ILogger<EFTaskRepository> logger)
        {
            _dbOptions = dbOptions ?? throw new ArgumentNullException(nameof(dbOptions));
            _logger = logger;
        }

        private TaskDbContext GetContext() => new TaskDbContext(_dbOptions);

        public async Task<TaskData> AddAsync(TaskData task, CancellationToken token = default)
        {
            using var context = GetContext();

            var data = await context.Tasks.AddAsync(task, token);
            _logger.LogInformation($"Task {task.Id} added");

            await context.SaveChangesAsync(token);

            return data.Entity;
        }

        public Task<TaskData> GetAsync(int id, CancellationToken token = default)
        {
            using var context = GetContext();

            return context.Tasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<TaskData>> GetAsync(Expression<Func<TaskData, bool>> predicate, int count = -1, CancellationToken token = default)
        {
            using var context = GetContext();

            var query = context.Tasks.AsNoTracking();
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
            using var context = GetContext();

            TaskData task = await context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (task != null)
            {
                context.Tasks.Remove(task);
                await context.SaveChangesAsync(token);
            }
        }

        public async Task<TaskData> UpdateAsync(TaskData task, CancellationToken token = default)
        {
            using var context = GetContext();

            TaskData dbtask = await context.Tasks.FirstOrDefaultAsync(x => x.Id == task.Id);
            context.Entry(dbtask).CurrentValues.SetValues(task);

            await context.SaveChangesAsync();

            return dbtask;
        }
    }
}
