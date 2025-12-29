using System.Security.Claims;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserInterface", Version = "1.0")]

namespace Nullbox.Security.Application.Common.Interfaces;

public interface ICurrentUser
{
    string? Id { get; }
    string? Name { get; }
    [IntentIgnore]
    string? Email { get; }
    [IntentIgnore]
    string? Audience { get; }
    [IntentIgnore]
    string? Issuer { get; }
    ClaimsPrincipal Principal { get; }
}