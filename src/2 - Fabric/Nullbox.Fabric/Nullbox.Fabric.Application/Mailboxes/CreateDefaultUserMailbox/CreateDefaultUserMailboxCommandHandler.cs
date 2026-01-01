using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Extensions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Entities.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;
using Nullbox.Fabric.Domain.Services.NumberGenerator;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateDefaultUserMailbox;

public class CreateDefaultUserMailboxCommandHandler : IRequestHandler<CreateDefaultUserMailboxCommand>
{
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IMailboxRoutingKeyMapRepository _routingKeyMapRepository;
    private readonly IUniqueIdentifierGenerator _uniqueIdentifierGenerator;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public CreateDefaultUserMailboxCommandHandler(
        IMailboxRepository mailboxRepository,
        IMailboxRoutingKeyMapRepository routingKeyMapRepository,
        IUniqueIdentifierGenerator uniqueIdentifierGenerator,
        IPartitionKeyScope partitionKeyScope)
    {
        _mailboxRepository = mailboxRepository;
        _routingKeyMapRepository = routingKeyMapRepository;
        _uniqueIdentifierGenerator = uniqueIdentifierGenerator;
        _partitionKeyScope = partitionKeyScope;
    }

    [IntentIgnore]
    public async Task Handle(CreateDefaultUserMailboxCommand request, CancellationToken cancellationToken)
    {
        using var _ = _partitionKeyScope.Push(request.Id.ToString());

        var routingKey = await _uniqueIdentifierGenerator.GenerateAsync(
            new UniqueIdentifierGeneratorSettings()
            {
                Length = 6,
                Chars = "ABCDEFHJKMNPQRSTUVWXYZ23456789"
            },
            async (key) =>
            {
                var existing = await _routingKeyMapRepository.FindByIdAsync(key, cancellationToken);
                return existing == null;
            });

        var defaultUserProfile = request.Users[0];

        var mailbox = new Mailbox(
            accountId: request.Id,
            routingKey: routingKey.ToLowerInvariant(),
            domain: "nullbox.email",
            name: $"{request.Name}'s mailbox",
            autoCreateAlias: true,
            emailAddress: defaultUserProfile.EmailAddress);

        mailbox.AddUser(
            userProfileId: defaultUserProfile.UserProfileId,
            roleId: defaultUserProfile.RoleId);

        _mailboxRepository.Add(mailbox);
    }
}