using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Security.Application.Users.OnboardUser;

public class OnboardUserCommand : IRequest, ICommand
{
    public OnboardUserCommand(string name, string? remoteIp, string cfTurnstileResponse)
    {
        Name = name;
        RemoteIp = remoteIp;
        CfTurnstileResponse = cfTurnstileResponse;
    }

    public string Name { get; set; }
    public string? RemoteIp { get; set; }
    public string CfTurnstileResponse { get; set; }
}