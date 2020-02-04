using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChangeLog.API.Services.Core
{
    public interface ITaskRepositoryFactory
    {
        ITaskRepository GetRepository();
    }
}
