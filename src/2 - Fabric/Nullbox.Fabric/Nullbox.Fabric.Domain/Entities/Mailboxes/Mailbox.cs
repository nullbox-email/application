using System.Net.Mail;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Entities.Accounts;
using Nullbox.Fabric.Domain.Events.Mailboxes;
using Nullbox.Fabric.Domain.ValueObjects;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Mailboxes;

public partial class Mailbox
{
    public Mailbox(Guid accountId, string routingKey, string domain, string name, bool autoCreateAlias, string emailAddress)
    {
        AccountId = accountId;
        RoutingKey = routingKey;
        Domain = domain;
        Name = name;
        AutoCreateAlias = autoCreateAlias;

        EmailAddress = emailAddress;

        DomainEvents.Add(new MailboxCreatedDomainEvent(
            mailbox: this));
    }

    public void Update(string name, bool autoCreateAlias, string emailAddress)
    {
        Name = name;

        AutoCreateAlias = autoCreateAlias;
        EmailAddress = emailAddress;
        DomainEvents.Add(new MailboxUpdatedDomainEvent(
            mailbox: this));
    }

    public void AddUser(Guid userProfileId, Guid roleId)
    {
        if (userProfileId == Guid.Empty)
        {
            throw new ArgumentException("UserProfileId cannot be empty.", nameof(userProfileId));
        }

        if (_users.Any(u => u.UserProfileId == userProfileId))
        {
            throw new ConflictException($"User with UserProfileId {userProfileId} is already associated with this mailbox.");
        }

        _users.Add(new MailboxUser(userProfileId, roleId));
    }

    public void RemoveUser(Guid userProfileId)
    {
        if (!_users.Any(u => u.UserProfileId == userProfileId))
        {
            throw new NotFoundException("No user with the specified UserProfileId is associated with this mailbox.");
        }

        _users.RemoveAll(u => u.UserProfileId == userProfileId);
    }
}