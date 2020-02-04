using ChangeLog.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeLog.API.Services.Core
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskData>> GetTasks(bool activeOnly, int count);

        Task<TaskData> GetTask(int taskId);

        Task<TaskData> AddTask(string createdBy, TaskPayload payload);

        Task<bool> RemoveTask(int taskId);

        Task<bool> CancelTask(int taskId);
    }
}
