using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Accounts.CreateAccount;

public class CreateAccountCommand : IRequest<Guid>, ICommand
{
    public CreateAccountCommand(Guid id, string name, string emailAddress)
    {
        Id = id;
        Name = name;
        EmailAddress = emailAddress;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string EmailAddress { get; set; }
}