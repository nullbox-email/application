using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Infrastructure.Persistence;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbInitializationExtensions", Version = "1.0")]

namespace Nullbox.Security.Api.StartupJobs;

public static class DbInitializationExtensions
{
    /// <summary>
    /// Performs a check to see whether the database exist and if not will create it
    /// based on the EntityFrameworkCore DbContext configuration.
    /// </summary>
    public static async Task EnsureDbCreationAsync(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        if (dbContext == null)
        {
            throw new InvalidOperationException("DbContext not configured in Services Collection in order to ensure that the database is created.");
        }

        await dbContext.EnsureDbCreatedAsync();
    }
}