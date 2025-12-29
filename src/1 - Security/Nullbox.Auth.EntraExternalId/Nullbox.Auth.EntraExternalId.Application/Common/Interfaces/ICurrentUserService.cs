using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Identity.CurrentUserServiceInterface", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Task<ICurrentUser?> GetAsync();
    Task<bool> IsInRoleAsync(string role);
    Task<bool> AuthorizeAsync(string policy);
}