using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nullbox.Security.Application.Common.Eventing;
using Nullbox.Security.Infrastructure.Eventing;

[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace Nullbox.Security.Infrastructure.Configuration;

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
            x.UsingRabbitMq((context, cfg) =>
            {
                // [IntentIgnore]
                ushort port = ushort.Parse(configuration["RabbitMq:Port"]);

                // [IntentIgnore]
                cfg.Host(configuration["RabbitMq:Host"], port, configuration["RabbitMq:VirtualHost"], host =>
                {
                    host.Username(configuration["RabbitMq:Username"]);
                    host.Password(configuration["RabbitMq:Password"]);
                });

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