using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace Nullbox.Fabric.Aliases.Eventing.Messages.Aliases;

public record AliasCreatedLearningModeScheduleV1
{
    public AliasCreatedLearningModeScheduleV1()
    {
        Name = null!;
        LocalPart = null!;
    }

    public Guid Id { get; init; }
    public Guid MailboxId { get; init; }
    public Guid AccountId { get; init; }
    public string Name { get; init; }
    public string LocalPart { get; init; }
    public bool IsEnabled { get; init; }
    public bool DirectPassthrough { get; init; }
    public bool LearningMode { get; init; }
}