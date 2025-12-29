using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandlerInterface", Version = "1.0")]

namespace Nullbox.Security.Application.Common.Eventing;

public interface IIntegrationEventHandler<in TMessage>
        where TMessage : class
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}