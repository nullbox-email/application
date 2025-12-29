using Intent.RoslynWeaver.Attributes;
using Nullbox.Auth.EntraExternalId.Domain.Common;

[assembly: IntentTemplate("Intent.DomainEvents.DomainEventServiceInterface", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}