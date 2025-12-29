using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Security.Application.Users.OnboardUser;

public class OnboardUserCommand : IRequest, ICommand
{
    public OnboardUserCommand(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}