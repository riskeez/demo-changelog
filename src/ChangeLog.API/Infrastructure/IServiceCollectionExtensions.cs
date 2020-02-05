using ChangeLog.API.Services;
using ChangeLog.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChangeLog.API.Infrastructure
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryTaskRepository(this IServiceCollection services)
        {
            return services.RemoveAll<ITaskRepository>()
                .AddSingleton<ITaskRepository, InMemoryTaskRepository>();
        }
    }
}
