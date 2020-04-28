using System;

namespace ChangeLog.API.Infrastructure
{
    public class BackgroundTaskSettings
    {
        public bool Enabled { get; set; } = true;
        public int Period { get; set; } = 30000;
    }
}
