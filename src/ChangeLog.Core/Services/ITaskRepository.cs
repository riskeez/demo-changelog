using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeLog.Core.Services
{
    public interface ITaskRepository
    {
        Task<TaskData> GetAsync(int id, CancellationToken token = default);
        Task<IEnumerable<TaskData>> GetAsync(Expression<Func<TaskData, bool>> predicate, int count = -1, CancellationToken token = default);
        Task<TaskData> AddAsync(TaskData task, CancellationToken token = default);
        Task<TaskData> UpdateAsync(TaskData task, CancellationToken token = default);
        Task RemoveAsync(int id, CancellationToken token = default);
    }
}
