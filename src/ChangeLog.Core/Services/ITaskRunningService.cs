using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeLog.Core.Services
{
    public interface ITaskRunningService
    {
        Task<ExecutionResult> TryRunTask();
        Task<ExecutionResult> TryStopTask(int taskId);
    }
}
