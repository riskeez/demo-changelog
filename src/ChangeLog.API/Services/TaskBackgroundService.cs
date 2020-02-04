using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ChangeLog.API.Infrastructure.Settings;
using ChangeLog.Core.Services;

namespace ChangeLog.API.Services
{
    /// <summary>
    /// Running as background process to make sure that planned tasks will be executed
    /// </summary>
    public class TaskBackgroundService : BackgroundService
    {
        private readonly BackgroundTaskSettings _settings;
        private readonly ITaskRunningService _taskRunner;
        private readonly ILogger _logger;

        public TaskBackgroundService(IOptions<BackgroundTaskSettings> options, ITaskRunningService taskRunner, ILogger<TaskBackgroundService> logger)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(BackgroundTaskSettings));
            _taskRunner = taskRunner ?? throw new ArgumentNullException(nameof(ITaskRunningService));
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

                _ = _taskRunner.TryRunTask();

                await Task.Delay(_settings.Period);
            }
        }
    }
}
