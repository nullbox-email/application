using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Extensions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Entities.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;
using Nullbox.Fabric.Domain.Services.NumberGenerator;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateDefaultUserMailbox;

public class CreateDefaultUserMailboxCommandHandler : IRequestHandler<CreateDefaultUserMailboxCommand>
{
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IMailboxRoutingKeyMapRepository _routingKeyMapRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUniqueIdentifierGenerator _uniqueIdentifierGenerator;

    public CreateDefaultUserMailboxCommandHandler(
        IMailboxRepository mailboxRepository,
        IMailboxRoutingKeyMapRepository routingKeyMapRepository,
        ICurrentUserService currentUserService,
        IUniqueIdentifierGenerator uniqueIdentifierGenerator)
    {
        _mailboxRepository = mailboxRepository;
        _routingKeyMapRepository = routingKeyMapRepository;
        _currentUserService = currentUserService;
        _uniqueIdentifierGenerator = uniqueIdentifierGenerator;
    }

    [IntentIgnore]
    public async Task Handle(CreateDefaultUserMailboxCommand request, CancellationToken cancellationToken)
    {
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

        _mailboxRepository.Add(mailbox);
    }
}