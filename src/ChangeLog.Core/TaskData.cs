using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChangeLog.Core
{
    public enum TaskState : int
    {
        Planned = 0,
        Active = 100,
        Finished = 200,
        Cancelled = 300,
        Error = 400
    }

    public class TaskData
    {
        public int Id { get; set; }
        public TaskState Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? FinishedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CancelledBy { get; set; }
        public int PercentComplete { get; set; }
        public TaskPayload Payload { get; set; }
    }

    public class TaskPayload
    {
        public string TargetBranch { get; set; }
        public string FromVersion { get; set; }
        public string ToVersion { get; set; }
    }
}