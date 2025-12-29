using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.MessageBusInterface", Version = "1.0")]

namespace Nullbox.Security.Application.Common.Eventing;

/// <summary>
/// Provides an abstraction for dispatching messages to one or more underlying message brokers.
/// Messages are queued via <see cref="Publish{TMessage}"/> (fan-out / broadcast semantics) or <see cref="Send{TMessage}"/> (point-to-point semantics) and are flushed in batches when <see cref="FlushAllAsync"/> is invoked.
/// </summary>
/// <remarks>
/// Implementations typically buffer messages to optimize throughput; you must call <see cref="FlushAllAsync"/> to ensure queued messages are actually dispatched.
/// Overloads that accept <c>IDictionary&lt;string, object&gt; additionalData</c> allow passing provider-specific metadata (headers, correlation identifiers, routing keys, scheduling information, explicit destination addresses, etc.).
/// Scheduling of messages (delayed or at a specific time) and explicit address targeting are provider-specific capabilities introduced by implementations such as MassTransit, surfaced via additional overloads.
/// </remarks>
public interface IMessageBus
{
    /// <summary>
    /// Queues a message to be published to all interested subscribers (topic / fan-out semantics).
    /// </summary>
    /// <typeparam name="TMessage">The concrete message type.</typeparam>
    /// <param name="message">The message instance to publish.</param>
    /// <remarks>
    /// The message is buffered until <see cref="FlushAllAsync"/> is called.
    /// </remarks>
    void Publish<TMessage>(TMessage message)
        where TMessage : class;
    /// <summary>
    /// Queues a message to be published including provider-specific metadata.
    /// </summary>
    /// <typeparam name="TMessage">The concrete message type.</typeparam>
    /// <param name="message">The message instance to publish.</param>
    /// <param name="additionalData">Arbitrary provider-specific metadata (e.g. headers, correlation ids, routing keys, scheduling info).</param>
    /// <remarks>
    /// Scheduling related entries (e.g. a scheduled date) are interpreted only by providers that support scheduling (such as MassTransit).
    /// </remarks>
    void Publish<TMessage>(TMessage message, IDictionary<string, object> additionalData)
        where TMessage : class;
    /// <summary>
    /// Queues a message for point-to-point delivery to a single consumer endpoint.
    /// </summary>
    /// <typeparam name="TMessage">The concrete message type.</typeparam>
    /// <param name="message">The message instance to send.</param>
    /// <remarks>
    /// The message is buffered until <see cref="FlushAllAsync"/> is called. Use send for commands or direct messages; use publish for events.
    /// </remarks>
    void Send<TMessage>(TMessage message)
        where TMessage : class;
    /// <summary>
    /// Queues a point-to-point message including provider-specific metadata.
    /// </summary>
    /// <typeparam name="TMessage">The concrete message type.</typeparam>
    /// <param name="message">The message instance to send.</param>
    /// <param name="additionalData">Arbitrary provider-specific metadata (e.g. headers, correlation ids, explicit address, scheduling info).</param>
    /// <remarks>
    /// An explicit destination address (when supported by the underlying provider such as MassTransit) or scheduling information may be included in <paramref name="additionalData"/>.
    /// </remarks>
    void Send<TMessage>(TMessage message, IDictionary<string, object> additionalData)
        where TMessage : class;
    /// <summary>
    /// Queues a point-to-point message for dispatch to a specific broker address.
    /// </summary>
    /// <typeparam name="TMessage">The concrete message type.</typeparam>
    /// <param name="message">The message instance to send.</param>
    /// <param name="address">The destination address understood by providers that support explicit addressing (MassTransit-specific concept).</param>
    /// <remarks>
    /// The message is buffered until <see cref="FlushAllAsync"/> is invoked. Providers that do not support explicit addressing may ignore this overload.
    /// </remarks>
    void Send<TMessage>(TMessage message, Uri address)
        where TMessage : class;
    /// <summary>
    /// Flushes and dispatches all queued messages to the underlying broker(s).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <remarks>
    /// Implementations may optimize by batching publish/send operations. Internal buffers are typically cleared after a successful flush.
    /// </remarks>
    Task FlushAllAsync(CancellationToken cancellationToken = default);
}