using ChangeLog.API.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeLog.API.Services.Core
{
    public interface ITaskRunningService
    {
        Task<ExecutionResult> TryRunTask();
        Task<ExecutionResult> TryStopTask(int taskId);
    }
}
