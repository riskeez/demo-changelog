using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeLog.Core
{
    public class ActiveTaskData : IDisposable
    {
        public void Init(TaskData task)
        {
            Data = task;
            CancellationTokenSource = new CancellationTokenSource();
        }

        public TaskData Data { get; private set; }

        public CancellationTokenSource CancellationTokenSource { get; private set; }

        public CancellationToken Token => CancellationTokenSource.Token;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        private void Dispose(bool isDisposing)
        {
            if (!isDisposing) return;

            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                CancellationTokenSource.Dispose();
                CancellationTokenSource = null;
            }
        }
    }
}
