using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.IntegrationEvents.Users;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateDefaultUserMailbox;

public class CreateDefaultUserMailboxCommand : IRequest, ICommand
{
    public CreateDefaultUserMailboxCommand(Guid id,
            string name,
            string domain,
            List<CreateDefaultUserMailboxCommandUsersDto> users)
    {
        Id = id;
        Name = name;
        Domain = domain;
        Users = users;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Domain { get; set; }
    public List<CreateDefaultUserMailboxCommandUsersDto> Users { get; set; }
}