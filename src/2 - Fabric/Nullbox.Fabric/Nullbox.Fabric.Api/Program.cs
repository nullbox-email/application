using System.Text.Json.Serialization;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Api;
using Nullbox.Fabric.Api.Configuration;
using Nullbox.Fabric.Api.Filters;
using Nullbox.Fabric.Api.Logging;
using Nullbox.Fabric.Api.StartupJobs;
using Nullbox.Fabric.Application;
using Nullbox.Fabric.Infrastructure;
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
    builder.Services.ConfigureHostedServices();
    builder.Services.AddApplication(builder.Configuration);
    builder.Services.ConfigureApplicationSecurity(builder.Configuration);
    builder.Services.ConfigureCors(builder.Configuration);
    builder.Services.ConfigureHealthChecks(builder.Configuration);
    builder.Services.ConfigureProblemDetails();
    builder.Services.ConfigureApiVersioning();
    builder.Services.AddInfrastructure(builder.Configuration);
    // [IntentIgnore]
    builder.Services.ConfigureNumberGenerator();
    builder.Services.AddTelemetryConfiguration(builder.Configuration);
    builder.Services.ConfigureQuartz(builder.Configuration);
    builder.Services.ConfigureOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    // [IntentIgnore]
    app.Use(async (ctx, next) =>
    {
        ctx.Request.EnableBuffering();
        using var ms = new MemoryStream();
        await ctx.Request.Body.CopyToAsync(ms);
        ctx.Items["RawBodyBytes"] = ms.ToArray();
        ctx.Request.Body.Position = 0;
        await next();
    });

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapScalarApiReference();
    app.MapOpenApi();
    app.MapDefaultHealthChecks();

    // [IntentIgnore]
    if (builder.Configuration.GetValue<bool>("Cosmos:EnsureDbCreated"))
    {
        app.EnsureDbCreationAsync().GetAwaiter().GetResult();
    }

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
