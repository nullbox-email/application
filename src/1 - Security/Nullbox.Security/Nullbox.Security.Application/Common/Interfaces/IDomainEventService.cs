using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Common;

[assembly: IntentTemplate("Intent.DomainEvents.DomainEventServiceInterface", Version = "1.0")]

namespace Nullbox.Security.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}