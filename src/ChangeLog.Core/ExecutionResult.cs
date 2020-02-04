using System;

namespace ChangeLog.Core
{
    public enum ExecuteResult: int
    {
        NoResult = 0,
        Success = 100,
        Cancelled = 200,
        Error = 300
    }

    public class ExecutionResult
    {
        public int? TaskId { get; set; }
        public ExecuteResult Result { get; set; }
    }
}
