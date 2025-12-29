using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Aryzac.Security.ScopePermissionMap", Version = "1.0")]

namespace Nullbox.Fabric.Application.Security;

public class Scopes
{
    private List<Scope> _scopes = new List<Scope>();

    public Scopes()
    {
        _scopes.Add(new Scope(Name: "nullbox.mailbox.update", Permissions: [typeof(Nullbox.Fabric.Application.Mailboxes.AliasMapUpdateMailbox.AliasMapUpdateMailboxCommand), typeof(Nullbox.Fabric.Application.Mailboxes.MailboxMapUpdateMailbox.MailboxMapUpdateMailboxCommand), typeof(Nullbox.Fabric.Application.Mailboxes.UpdateMailbox.UpdateMailboxCommand)]));
        _scopes.Add(new Scope(Name: "nullbox.account.create", Permissions: [typeof(Nullbox.Fabric.Application.Accounts.CreateAccount.CreateAccountCommand)]));
        _scopes.Add(new Scope(Name: "nullbox.alias.create", Permissions: [typeof(Nullbox.Fabric.Application.Aliases.CreateAlias.CreateAliasCommand), typeof(Nullbox.Fabric.Application.Aliases.CreateAliasMap.CreateAliasMapCommand)]));
        _scopes.Add(new Scope(Name: "nullbox.mailbox.create", Permissions: [typeof(Nullbox.Fabric.Application.Mailboxes.CreateDefaultUserMailbox.CreateDefaultUserMailboxCommand), typeof(Nullbox.Fabric.Application.Mailboxes.CreateMailbox.CreateMailboxCommand), typeof(Nullbox.Fabric.Application.Mailboxes.CreateMailboxMap.CreateMailboxMapCommand), typeof(Nullbox.Fabric.Application.Mailboxes.CreateMailboxRoutingKeyMap.CreateMailboxRoutingKeyMapCommand)]));
        _scopes.Add(new Scope(Name: "nullbox.statistics.update", Permissions: [typeof(Nullbox.Fabric.Application.Statistics.ProcessActivities.ProcessActivitiesCommand), typeof(Nullbox.Fabric.Application.Statistics.ProcessRollups.ProcessRollupsCommand), typeof(Nullbox.Fabric.Application.Statistics.ProcessStatistics.ProcessStatisticsCommand)]));
        _scopes.Add(new Scope(Name: "nullbox.delivery-action.create", Permissions: [typeof(Nullbox.Fabric.Application.Deliveries.ProcessEmail.ProcessEmailCommand)]));
        _scopes.Add(new Scope(Name: "nullbox.delivery-action.complete", Permissions: [typeof(Nullbox.Fabric.Application.Deliveries.ProcessEmailComplete.ProcessEmailCompleteCommand)]));
        _scopes.Add(new Scope(Name: "nullbox.delivery-action.quarantine", Permissions: [typeof(Nullbox.Fabric.Application.Deliveries.QuarantineEmail.QuarantineEmailCommand)]));
        _scopes.Add(new Scope(Name: "nullbox.alias.update", Permissions: [typeof(Nullbox.Fabric.Application.Aliases.UpdateAliasMap.UpdateAliasMapCommand)]));
        _scopes.Add(new Scope(Name: "nullbox.alias.read", Permissions: [typeof(Nullbox.Fabric.Application.Aliases.GetAliasById.GetAliasByIdQuery)]));
        _scopes.Add(new Scope(Name: "nullbox.alias.read-all", Permissions: [typeof(Nullbox.Fabric.Application.Aliases.GetAliases.GetAliasesQuery)]));
        _scopes.Add(new Scope(Name: "nullbox.dashboard.read-all", Permissions: [typeof(Nullbox.Fabric.Application.Dashboards.GetDashboard.GetDashboardQuery)]));
        _scopes.Add(new Scope(Name: "nullbox.mailbox.read", Permissions: [typeof(Nullbox.Fabric.Application.Mailboxes.GetUserMailboxByRoutingKeyAndDomain.GetUserMailboxByRoutingKeyAndDomainQuery)]));
        _scopes.Add(new Scope(Name: "nullbox.mailbox.read-all", Permissions: [typeof(Nullbox.Fabric.Application.Mailboxes.GetUserMailboxes.GetUserMailboxesQuery)]));
    }

    public IReadOnlyList<Scope> All => _scopes;
}

public record Scope(string Name, List<Type> Permissions)
{
    public override string ToString()
    {
        return $"{Name}";
    }
}