using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Api.Jobs;
using Quartz;

[assembly: IntentTemplate("Intent.QuartzScheduler.QuartzConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Api.Configuration;

public static class QuartzConfiguration
{
    public static IServiceCollection ConfigureQuartz(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddQuartz(q =>
            {
                if (configuration.GetValue<bool?>($"Quartz:Jobs:DisableLearningModeOnNewAliasesJob:Enabled") ?? true)
                {
                    q.ScheduleJob<DisableLearningModeOnNewAliasesJob>(trigger =>
                    {
                        trigger.WithCronSchedule(configuration.GetValue<string?>($"Quartz:Jobs:DisableLearningModeOnNewAliasesJob:CronSchedule") ?? "0 0 0 * * ?");
                        trigger.WithIdentity("DisableLearningModeOnNewAliasesJob");
                    });
                }
            });

        services
            .AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

        return services;
    }
}