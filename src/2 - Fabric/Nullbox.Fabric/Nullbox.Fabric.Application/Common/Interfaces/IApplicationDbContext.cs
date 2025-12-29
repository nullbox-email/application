using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Nullbox.Fabric.Domain.Entities.Accounts;
using Nullbox.Fabric.Domain.Entities.Activities;
using Nullbox.Fabric.Domain.Entities.Aliases;
using Nullbox.Fabric.Domain.Entities.Deliveries;
using Nullbox.Fabric.Domain.Entities.Mailboxes;
using Nullbox.Fabric.Domain.Entities.Markers;
using Nullbox.Fabric.Domain.Entities.Products;
using Nullbox.Fabric.Domain.Entities.Rollups;
using Nullbox.Fabric.Domain.Entities.Statistics;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace Nullbox.Fabric.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
    DbSet<AccountRole> AccountRoles { get; }
    DbSet<AccountUserMap> AccountUserMaps { get; }
    DbSet<EffectiveEnablement> EffectiveEnablements { get; }
    DbSet<EnablementGrant> EnablementGrants { get; }
    DbSet<RecentDeliveryAction> RecentDeliveryActions { get; }
    DbSet<Alias> Aliases { get; }
    DbSet<AliasLearningModeSchedule> AliasLearningModeSchedules { get; }
    DbSet<AliasMap> AliasMaps { get; }
    DbSet<AliasRule> AliasRules { get; }
    DbSet<AliasSender> AliasSenders { get; }
    DbSet<AliasSenderDecision> AliasSenderDecisions { get; }
    DbSet<DeliveryAction> DeliveryActions { get; }
    DbSet<Mailbox> Mailboxes { get; }
    DbSet<MailboxMap> MailboxMaps { get; }
    DbSet<MailboxRoutingKeyMap> MailboxRoutingKeyMaps { get; }
    DbSet<AppliedMarker> AppliedMarkers { get; }
    DbSet<ProductDefinition> ProductDefinitions { get; }
    DbSet<TopAlias> TopAliases { get; }
    DbSet<TopDomain> TopDomains { get; }
    DbSet<TrafficStatistic> TrafficStatistics { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}