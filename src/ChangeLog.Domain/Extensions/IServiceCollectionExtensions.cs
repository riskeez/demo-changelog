using ChangeLog.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace ChangeLog.Domain
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEFTaskRepository(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            var optBuilder = new DbContextOptionsBuilder<TaskDbContext>();
            optionsAction(optBuilder);

            return services.RemoveAll<ITaskRepository>()
                .AddTransient(x => optBuilder.Options)
                .AddTransient<ITaskRepository, EFTaskRepository>();
        }
    }
}
