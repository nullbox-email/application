using System.Text.Json.Serialization;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Auth.EntraExternalId.Api;
using Nullbox.Auth.EntraExternalId.Api.Configuration;
using Nullbox.Auth.EntraExternalId.Api.Filters;
using Nullbox.Auth.EntraExternalId.Api.Logging;
using Nullbox.Auth.EntraExternalId.Application;
using Nullbox.Auth.EntraExternalId.Infrastructure;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

using var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    if (builder.Configuration.GetValue<bool?>("KeyVault:Enabled") == true)
    {
        builder.Configuration.ConfigureAzureKeyVault(builder.Configuration);
    }

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Destructure.With(new BoundedLoggingDestructuringPolicy()));

    builder.Services.AddControllers(
        opt =>
        {
            opt.Filters.Add<ExceptionFilter>();
        })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    builder.Services.AddApplication(builder.Configuration);
    builder.Services.ConfigureApplicationSecurity(builder.Configuration);
    builder.Services.ConfigureHealthChecks(builder.Configuration);
    builder.Services.ConfigureProblemDetails();
    builder.Services.ConfigureApiVersioning();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddTelemetryConfiguration(builder.Configuration);
    builder.Services.ConfigureOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapScalarApiReference();
    app.MapOpenApi();
    app.MapDefaultHealthChecks();
    app.MapControllers();

    logger.Write(LogEventLevel.Information, "Starting web host");

    app.Run();
}
catch (HostAbortedException)
{
    // Excluding HostAbortedException from being logged, as this is an expected
    // exception when working with EF Core migrations (as per the .NET team on the below link)
    // https://github.com/dotnet/efcore/issues/29809#issuecomment-1344101370
}
catch (Exception ex)
{
    logger.Write(LogEventLevel.Fatal, ex, "Unhandled exception");
}
