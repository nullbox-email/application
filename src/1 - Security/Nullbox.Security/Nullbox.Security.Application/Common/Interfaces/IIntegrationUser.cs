using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Endura.IntegrationUserInterface", Version = "1.0")]

namespace Nullbox.Security.Application.Common.Interfaces;

public interface IIntegrationUser
{
    Guid UserId { get; }
    void SetUser(Guid UserId);
}