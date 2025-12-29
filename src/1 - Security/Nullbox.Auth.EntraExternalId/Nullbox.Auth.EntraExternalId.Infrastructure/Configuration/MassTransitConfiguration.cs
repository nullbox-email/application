using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nullbox.Auth.EntraExternalId.Application.Common.Eventing;
using Nullbox.Auth.EntraExternalId.Infrastructure.Eventing;

[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Infrastructure.Configuration;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMassTransitConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddScoped<MassTransitMessageBus>();
        services.AddScoped<IMessageBus>(provider => provider.GetRequiredService<MassTransitMessageBus>());

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumers();
            x.UsingInMemory((context, cfg) =>
            {
                cfg.UseMessageRetry(r => r.Interval(
                    configuration.GetValue<int?>("MassTransit:RetryInterval:RetryCount") ?? 10,
                    configuration.GetValue<TimeSpan?>("MassTransit:RetryInterval:Interval") ?? TimeSpan.FromSeconds(5)));

                cfg.ConfigureEndpoints(context);
                cfg.UseInMemoryOutbox(context);
            });
            x.AddInMemoryInboxOutbox();
        });
        return services;
    }

    private static void AddConsumers(this IRegistrationConfigurator cfg)
    {
    }
}