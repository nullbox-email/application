using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Quartz;

[assembly: IntentTemplate("Intent.QuartzScheduler.ScheduledJob", Version = "1.0")]

namespace Nullbox.Fabric.Api.Jobs;

[DisallowConcurrentExecution]
public class DisableLearningModeOnNewAliasesJob : IJob
{
    private readonly IAliasLearningModeScheduleRepository _aliasLearningModeScheduleRepository;
    private readonly IAliasRepository _aliasRepository;

    public DisableLearningModeOnNewAliasesJob(
        IAliasLearningModeScheduleRepository aliasLearningModeScheduleRepository,
        IAliasRepository aliasRepository
        )
    {
        _aliasLearningModeScheduleRepository = aliasLearningModeScheduleRepository;
        _aliasRepository = aliasRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // Learning window end: due in 30 days (UTC)
        var dateUtc = DateTimeOffset.UtcNow;

        // Partition bucket: yyyyMMdd (UTC) so the daily Quartz job can scan a single partition.
        var window = Keys.DueDateBucket(dateUtc);

        var aliasSchedules = await _aliasLearningModeScheduleRepository.FindAllAsync(a => a.Window == window && a.Status == Domain.Aliases.ScheduleStatus.Pending, default);

        foreach (var aliasSchedule in aliasSchedules)
        {
            var alias = await _aliasRepository.FindByIdAsync(aliasSchedule.AliasId, default);
            if (alias != null && alias.LearningMode)
            {
                alias.Update(alias.Name, alias.IsEnabled, alias.DirectPassthrough, learningMode: false);
                aliasSchedule.MarkAsProcessed();
            }
            else
            {
                aliasSchedule.MarkAsSkipped();
            }
        }
    }

    private static class Keys
    {
        public static string DueDateBucket(DateTimeOffset dueDateUtc)
            => dueDateUtc.ToUniversalTime().ToString("yyyyMMdd");
    }
}