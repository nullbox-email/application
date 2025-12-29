using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Caching.Distributed;

[assembly: IntentTemplate("Intent.AspNetCore.DistributedCaching.DistributedCacheWithUnitOfWorkInterface", Version = "1.0")]

namespace Nullbox.Security.Application.Common.Interfaces;

public interface IDistributedCacheWithUnitOfWork : IDistributedCache
{
    IDisposable EnableUnitOfWork();
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}