using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChangeLog.Core
{
    public class ChangelogData
    {
        public string BuildVersion { get; set; }
        public DateTimeOffset BuildDate { get; set; }
        public DateTimeOffset ConcurrencyToken { get; set; }
        public string Content { get; set; }
    }
}
