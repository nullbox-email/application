using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Aliases;

public partial class AliasLearningModeSchedule
{
    public AliasLearningModeSchedule(Guid id, string window, Guid aliasId, Guid mailboxId, DateTimeOffset dueDate)
    {
        Id = id;
        Window = window;
        AliasId = aliasId;
        MailboxId = mailboxId;
        DueDate = dueDate;
    }

    public void MarkAsProcessed()
    {
        // TODO: Implement MarkAsProcessed (AliasLearningModeSchedule) functionality
        throw new NotImplementedException("Replace with your implementation...");
    }

    public void MarkAsSkipped()
    {
        // TODO: Implement MarkAsSkipped (AliasLearningModeSchedule) functionality
        throw new NotImplementedException("Replace with your implementation...");
    }
}