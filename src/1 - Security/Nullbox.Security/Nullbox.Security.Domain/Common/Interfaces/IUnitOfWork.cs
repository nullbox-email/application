using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.UnitOfWorkInterface", Version = "1.0")]

namespace Nullbox.Security.Domain.Common.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}