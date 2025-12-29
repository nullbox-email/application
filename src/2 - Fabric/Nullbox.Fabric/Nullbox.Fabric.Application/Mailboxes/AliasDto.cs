using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes;

public class AliasDto
{
    public AliasDto()
    {
        Name = null!;
        LocalPart = null!;
    }

    public Guid Id { get; set; }
    public Guid MailboxId { get; set; }

    public string Name { get; set; }
    public string LocalPart { get; set; }
    public bool IsEnabled { get; set; }
    public bool DirectPassthrough { get; set; }
    public bool LearningMode { get; set; }

    public static AliasDto Create(
            Guid id,
            Guid mailboxId,
            string name,
            string localPart,
            bool isEnabled,
            bool directPassthrough,
            bool learningMode)
    {
        return new AliasDto
        {
            Id = id,
            MailboxId = mailboxId,
            Name = name,
            LocalPart = localPart,
            IsEnabled = isEnabled,
            DirectPassthrough = directPassthrough,
            LearningMode = learningMode
        };
    }
}