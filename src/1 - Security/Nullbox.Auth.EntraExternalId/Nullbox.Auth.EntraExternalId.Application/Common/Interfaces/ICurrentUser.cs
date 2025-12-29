using System.Security.Claims;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserInterface", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Application.Common.Interfaces;

public interface ICurrentUser
{
    string? Id { get; }
    string? Name { get; }
    ClaimsPrincipal Principal { get; }
}