using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Nullbox.Security.Domain.Entities.Tokens;
using Nullbox.Security.Domain.Entities.Users;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContextInterface", Version = "1.0")]

namespace Nullbox.Security.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Token> Tokens { get; }
    DbSet<ExternalUser> ExternalUsers { get; }
    DbSet<UserProfile> UserProfiles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}