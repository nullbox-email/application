using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Aliases.UpdateAliasMap;

public class UpdateAliasMapCommand : IRequest, ICommand
{
    public UpdateAliasMapCommand(string id,
            Guid aliasId,
            Guid mailboxId,
            Guid accountId,
            string name,
            string localPart,
            bool isEnabled,
            bool directPassthrough,
            bool learningMode)
    {
        Id = id;
        AliasId = aliasId;
        MailboxId = mailboxId;
        AccountId = accountId;
        Name = name;
        LocalPart = localPart;
        IsEnabled = isEnabled;
        DirectPassthrough = directPassthrough;
        LearningMode = learningMode;
    }

    public string Id { get; set; }
    public Guid AliasId { get; set; }
    public Guid MailboxId { get; set; }
    public Guid AccountId { get; set; }
    public string Name { get; set; }
    public string LocalPart { get; set; }
    public bool IsEnabled { get; set; }
    public bool DirectPassthrough { get; set; }
    public bool LearningMode { get; set; }
}