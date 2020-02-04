using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ChangeLog.API.Infrastructure.Settings;
using ChangeLog.API.Services;
using ChangeLog.Core.Services;

namespace ChangeLog.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Changelog Generator API", Version = "v1" });
            });

            services.Configure<BackgroundTaskSettings>(_configuration.GetSection("BackgroundTask"));
            services.Configure<TaskExecutionSettings>(_configuration.GetSection("Execution"));
            
            services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
            services.AddSingleton<ITaskExecutionPool, TaskExecutionPool>();
            services.AddSingleton<ITaskRunningService, TaskRunningService>();

            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IChangelogService, StubChangelogService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            string pathBase = _configuration.GetValue<string>("PathBase");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase)? pathBase : string.Empty) }/swagger/v1/swagger.json", "Changelog API v1");
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
