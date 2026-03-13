using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Aliases.DownloadQuarantinedEmlFile;

public class DownloadQuarantinedEmlFileQuery : IRequest<FileDownloadDto>, IQuery
{
    public DownloadQuarantinedEmlFileQuery(string mailboxId, string aliasId, string messageId)
    {
        MailboxId = mailboxId;
        AliasId = aliasId;
        MessageId = messageId;
    }

    public string MailboxId { get; set; }
    public string AliasId { get; set; }
    public string MessageId { get; set; }
}