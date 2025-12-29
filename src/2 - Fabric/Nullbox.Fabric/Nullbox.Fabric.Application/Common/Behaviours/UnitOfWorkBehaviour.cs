using System.Text.RegularExpressions;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Common.Interfaces;
using Nullbox.Fabric.Domain.Entities.Markers; // marker entity types

[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour", Version = "1.0")]

namespace Nullbox.Fabric.Application.Common.Behaviours;

/// <summary>
/// Ensures that all operations processed as part of handling a <see cref="ICommand"/> either
/// pass or fail as one unit. This behaviour makes it unnecessary for developers to call
/// SaveChangesAsync() inside their business logic (e.g. command handlers), and doing so should
/// be avoided unless absolutely necessary.
/// </summary>
public class UnitOfWorkBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, ICommand
{
    private readonly IDistributedCacheWithUnitOfWork _distributedCacheWithUnitOfWork;
    private readonly IUnitOfWork _dataSource;

    public UnitOfWorkBehaviour(IDistributedCacheWithUnitOfWork distributedCacheWithUnitOfWork, IUnitOfWork dataSource)
    {
        _distributedCacheWithUnitOfWork = distributedCacheWithUnitOfWork ?? throw new ArgumentNullException(nameof(distributedCacheWithUnitOfWork));
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
        {
            TResponse response;

            // The execution is wrapped in a transaction scope to ensure that if any other
            // SaveChanges calls to the data source (e.g. EF Core) are called, that they are
            // transacted atomically. The isolation is set to ReadCommitted by default (i.e. read-
            // locks are released, while write-locks are maintained for the duration of the
            // transaction). Learn more on this approach for EF Core:
            // https://docs.microsoft.com/en-us/ef/core/saving/transactions#using-systemtransactions
            // [IntentIgnore]
            using (var transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                response = await next(cancellationToken);

                // By calling SaveChanges at the last point in the transaction ensures that write-
                // locks in the database are created and then released as quickly as possible. This
                // helps optimize the application to handle a higher degree of concurrency.
                await SaveChangesWithCosmosMarkerConflictRetryAsync(_dataSource, cancellationToken);

                transaction.Complete();
            }

            await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);

            return response;
        }
    }

    [IntentIgnore]
    private static async Task SaveChangesWithCosmosMarkerConflictRetryAsync(IUnitOfWork uow, CancellationToken ct)
    {
        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (TryHandleMarkerConflict(uow, ex))
        {
            // One retry after detaching conflicting marker inserts.
            await uow.SaveChangesAsync(ct);
        }
    }

    [IntentIgnore]
    private static bool TryHandleMarkerConflict(IUnitOfWork uow, DbUpdateException ex)
    {
        // Must be Cosmos 409
        if (!IsCosmosConflict(ex)) return false;

        // We need ChangeTracker to detach conflicting Added entries.
        // If your IUnitOfWork is your DbContext (common in Intent), this will work.
        if (uow is not DbContext db) return false;

        var conflictingIds = ExtractConflictingIds(ex);
        if (conflictingIds.Count == 0) return false;

        var entries = db.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added && IsMarkerEntity(e.Entity))
            .ToList();

        var detachedAny = false;

        foreach (var entry in entries)
        {
            var id = GetEntityId(entry.Entity);
            if (id is null) continue;

            if (conflictingIds.Contains(id))
            {
                // This specific insert is a duplicate; drop it from the unit of work.
                entry.State = EntityState.Detached;
                detachedAny = true;
            }
        }

        return detachedAny;
    }

    [IntentIgnore]
    private static bool IsCosmosConflict(DbUpdateException dbu)
    {
        Exception? inner = dbu;
        while (inner is not null)
        {
            if (inner is CosmosException ce && ce.StatusCode == System.Net.HttpStatusCode.Conflict)
                return true;

            inner = inner.InnerException;
        }
        return false;
    }

    [IntentIgnore]
    private static HashSet<string> ExtractConflictingIds(DbUpdateException ex)
    {
        // Matches: "item with id '...'."
        // Your exceptions show exactly this text.
        var set = new HashSet<string>(StringComparer.Ordinal);
        var matches = Regex.Matches(ex.Message, @"id\s+'([^']+)'", RegexOptions.IgnoreCase);
        foreach (Match m in matches)
        {
            if (m.Success && m.Groups.Count > 1)
                set.Add(m.Groups[1].Value);
        }
        return set;
    }

    [IntentIgnore]
    private static bool IsMarkerEntity(object entity) =>
        entity is AppliedMarker;

    [IntentIgnore]
    private static string? GetEntityId(object entity)
    {
        // All your marker entities have `public string Id { get; set; }`
        var prop = entity.GetType().GetProperty("Id");
        return prop?.GetValue(entity) as string;
    }
}
