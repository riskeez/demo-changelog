using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ChangeLog.API.Infrastructure;
using ChangeLog.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeLog.API.Services
{
    /// <summary>
    /// Running as background process to make sure that planned tasks will be executed
    /// </summary>
    public class TaskBackgroundService : BackgroundService
    {
        private readonly BackgroundTaskSettings _settings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public TaskBackgroundService(IOptions<BackgroundTaskSettings> options, IServiceProvider serviceProvider, ILogger<TaskBackgroundService> logger)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(BackgroundTaskSettings));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(IServiceProvider));
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_settings.Enabled)
            {
                _logger.LogInformation("Background Service: Disabled");
                return;
            }

            _logger.LogInformation($"Background Service: Started. Execution period: {_settings.Period} ms.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace($"Background Service: Processing...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var taskRunner = scope.ServiceProvider.GetRequiredService<ITaskRunningService>();

                    // to keep scope alive we can't just fire & forget here
                    await taskRunner.TryRunTask();
                }

                await Task.Delay(_settings.Period);
            }
        }
    }
}
