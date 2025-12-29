using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Auth.EntraExternalId.Domain.Common;

[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventNotification", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Application.Common.Models;

public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
{
    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }

    public TDomainEvent DomainEvent { get; }
}