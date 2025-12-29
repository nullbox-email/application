using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Products.InitializeProductCatalog;

[assembly: IntentTemplate("Aryzac.HostedService", Version = "1.0")]

namespace Nullbox.Fabric.Api.Services;

public class RegisterProductsHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IServiceProvider _serviceProvider;

    public RegisterProductsHostedService(IHostApplicationLifetime appLifetime, IServiceProvider serviceProvider)
    {
        _appLifetime = appLifetime;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(async () => await OnStarted(cancellationToken));
        _appLifetime.ApplicationStopping.Register(async () => await OnStopping(cancellationToken));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    [IntentIgnore]
    private async Task OnStarted(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

            await mediator.Send(new InitializeProductCatalog(), cancellationToken);
        }
    }

    private async Task OnStopping(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
        }
    }
}