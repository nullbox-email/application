using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}