using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;
using Nullbox.Fabric.Aliases.Eventing.Messages.Aliases;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries;
using Nullbox.Fabric.Infrastructure.Eventing;
using Nullbox.Fabric.Mailboxes.Eventing.Messages.Mailboxes;
using Nullbox.Security.Users.Eventing.Messages.Users;

[assembly: IntentTemplate("Intent.Eventing.MassTransit.MassTransitConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Configuration;

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
                cfg.AddReceiveEndpoints(context);
                EndpointConventionRegistration();
            });
            x.AddInMemoryInboxOutbox();
        });
        return services;
    }

    private static void AddConsumers(this IRegistrationConfigurator cfg)
    {
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<AccountCreatedV1Event>, AccountCreatedV1Event>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<AccountCreatedV1Event>, AccountCreatedV1Event>)).Endpoint(config => config.InstanceId = "Nullbox-Fabric");

        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<AliasCreatedV1Event>, AliasCreatedV1Event>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<AliasCreatedV1Event>, AliasCreatedV1Event>)).Endpoint(config => config.InstanceId = "Nullbox-Fabric");

        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCompletedV1Event>, DeliveryActionCompletedV1Event>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionCompletedV1Event>, DeliveryActionCompletedV1Event>)).Endpoint(config => config.InstanceId = "Nullbox-Fabric");

        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCreatedV1Event>, DeliveryActionCreatedV1Event>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionCreatedV1Event>, DeliveryActionCreatedV1Event>)).Endpoint(config => config.InstanceId = "Nullbox-Fabric");

        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionDecisionedV1Event>, DeliveryActionDecisionedV1Event>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionDecisionedV1Event>, DeliveryActionDecisionedV1Event>)).Endpoint(config => config.InstanceId = "Nullbox-Fabric");
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<EnablementGrantCreatedV1Event>, EnablementGrantCreatedV1Event>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<EnablementGrantCreatedV1Event>, EnablementGrantCreatedV1Event>)).Endpoint(config => config.InstanceId = "Nullbox-Fabric");

        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<MailboxUpdatedV1Event>, MailboxUpdatedV1Event>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<MailboxUpdatedV1Event>, MailboxUpdatedV1Event>)).Endpoint(config => config.InstanceId = "Nullbox-Fabric");
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<AliasUpdatedV1Event>, AliasUpdatedV1Event>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<AliasUpdatedV1Event>, AliasUpdatedV1Event>)).Endpoint(config => config.InstanceId = "Nullbox-Fabric");
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<UserProfileActivatedV1Event>, UserProfileActivatedV1Event>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<UserProfileActivatedV1Event>, UserProfileActivatedV1Event>)).Endpoint(config => config.InstanceId = "Nullbox-Fabric");
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<AccountCreateAccountUserMapV1>, AccountCreateAccountUserMapV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<AccountCreateAccountUserMapV1>, AccountCreateAccountUserMapV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<AccountCreateDefaultUserMailboxV1>, AccountCreateDefaultUserMailboxV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<AccountCreateDefaultUserMailboxV1>, AccountCreateDefaultUserMailboxV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<AccountCreateEnablementV1>, AccountCreateEnablementV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<AccountCreateEnablementV1>, AccountCreateEnablementV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<AliasCreatedLearningModeScheduleV1>, AliasCreatedLearningModeScheduleV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<AliasCreatedLearningModeScheduleV1>, AliasCreatedLearningModeScheduleV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<CreateAliasMapV1>, CreateAliasMapV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<CreateAliasMapV1>, CreateAliasMapV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCompleteActivitiesV1>, DeliveryActionCompleteActivitiesV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionCompleteActivitiesV1>, DeliveryActionCompleteActivitiesV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCompleteRollupsV1>, DeliveryActionCompleteRollupsV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionCompleteRollupsV1>, DeliveryActionCompleteRollupsV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCompleteStatisticsV1>, DeliveryActionCompleteStatisticsV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionCompleteStatisticsV1>, DeliveryActionCompleteStatisticsV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCreateActivitiesV1>, DeliveryActionCreateActivitiesV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionCreateActivitiesV1>, DeliveryActionCreateActivitiesV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCreateRollupsV1>, DeliveryActionCreateRollupsV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionCreateRollupsV1>, DeliveryActionCreateRollupsV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCreateStatisticsV1>, DeliveryActionCreateStatisticsV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionCreateStatisticsV1>, DeliveryActionCreateStatisticsV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionDecisionActivitiesV1>, DeliveryActionDecisionActivitiesV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionDecisionActivitiesV1>, DeliveryActionDecisionActivitiesV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionDecisionRollupsV1>, DeliveryActionDecisionRollupsV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionDecisionRollupsV1>, DeliveryActionDecisionRollupsV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionDecisionStatisticsV1>, DeliveryActionDecisionStatisticsV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<DeliveryActionDecisionStatisticsV1>, DeliveryActionDecisionStatisticsV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<MailboxUpdateAliasMapV1>, MailboxUpdateAliasMapV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<MailboxUpdateAliasMapV1>, MailboxUpdateAliasMapV1>)).ExcludeFromConfigureEndpoints();
        cfg.AddConsumer<IntegrationEventConsumer<IIntegrationEventHandler<MailboxUpdateMailboxMapV1>, MailboxUpdateMailboxMapV1>>(typeof(IntegrationEventConsumerDefinition<IIntegrationEventHandler<MailboxUpdateMailboxMapV1>, MailboxUpdateMailboxMapV1>)).ExcludeFromConfigureEndpoints();
    }

    private static void AddReceiveEndpoints(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context)
    {
        cfg.ReceiveEndpoint("nullbox.fabric.accounts.eventing.messages.accounts.account-create-account-user-map-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<AccountCreateAccountUserMapV1>, AccountCreateAccountUserMapV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.accounts.eventing.messages.accounts.account-create-default-user-mailbox-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<AccountCreateDefaultUserMailboxV1>, AccountCreateDefaultUserMailboxV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.accounts.eventing.messages.accounts.account-create-enablement-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<AccountCreateEnablementV1>, AccountCreateEnablementV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.aliases.eventing.messages.aliases.alias-created-learning-mode-schedule-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<AliasCreatedLearningModeScheduleV1>, AliasCreatedLearningModeScheduleV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.aliases.eventing.messages.aliases.create-alias-map-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<CreateAliasMapV1>, CreateAliasMapV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-complete-activities-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCompleteActivitiesV1>, DeliveryActionCompleteActivitiesV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-complete-rollups-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCompleteRollupsV1>, DeliveryActionCompleteRollupsV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-complete-statistics-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCompleteStatisticsV1>, DeliveryActionCompleteStatisticsV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-create-activities-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCreateActivitiesV1>, DeliveryActionCreateActivitiesV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-create-rollups-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCreateRollupsV1>, DeliveryActionCreateRollupsV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-create-statistics-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionCreateStatisticsV1>, DeliveryActionCreateStatisticsV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-decision-activities-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionDecisionActivitiesV1>, DeliveryActionDecisionActivitiesV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-decision-rollups-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionDecisionRollupsV1>, DeliveryActionDecisionRollupsV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-decision-statistics-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<DeliveryActionDecisionStatisticsV1>, DeliveryActionDecisionStatisticsV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.mailboxes.eventing.messages.mailboxes.mailbox-update-alias-map-v1", e =>
            {
                e.ConfigureConsumeTopology = false;
                e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<MailboxUpdateAliasMapV1>, MailboxUpdateAliasMapV1>>(context);
            });
        cfg.ReceiveEndpoint("nullbox.fabric.mailboxes.eventing.messages.mailboxes.mailbox-update-mailbox-map-v1", e =>
        {
            e.ConfigureConsumeTopology = false;
            e.Consumer<IntegrationEventConsumer<IIntegrationEventHandler<MailboxUpdateMailboxMapV1>, MailboxUpdateMailboxMapV1>>(context);
        });
    }

    private static void EndpointConventionRegistration()
    {
        EndpointConvention.Map<Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries.DeliveryActionCompleteRollupsV1>(new Uri("queue:nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-complete-rollups-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Accounts.Eventing.Messages.Accounts.AccountCreateAccountUserMapV1>(new Uri("queue:nullbox.fabric.accounts.eventing.messages.accounts.account-create-account-user-map-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries.DeliveryActionDecisionActivitiesV1>(new Uri("queue:nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-decision-activities-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Accounts.Eventing.Messages.Accounts.AccountCreateDefaultUserMailboxV1>(new Uri("queue:nullbox.fabric.accounts.eventing.messages.accounts.account-create-default-user-mailbox-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries.DeliveryActionCompleteActivitiesV1>(new Uri("queue:nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-complete-activities-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Accounts.Eventing.Messages.Accounts.AccountCreateEnablementV1>(new Uri("queue:nullbox.fabric.accounts.eventing.messages.accounts.account-create-enablement-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries.DeliveryActionCreateRollupsV1>(new Uri("queue:nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-create-rollups-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Mailboxes.Eventing.Messages.Mailboxes.MailboxUpdateAliasMapV1>(new Uri("queue:nullbox.fabric.mailboxes.eventing.messages.mailboxes.mailbox-update-alias-map-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries.DeliveryActionCreateStatisticsV1>(new Uri("queue:nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-create-statistics-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries.DeliveryActionCompleteStatisticsV1>(new Uri("queue:nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-complete-statistics-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Mailboxes.Eventing.Messages.Mailboxes.MailboxUpdateMailboxMapV1>(new Uri("queue:nullbox.fabric.mailboxes.eventing.messages.mailboxes.mailbox-update-mailbox-map-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries.DeliveryActionCreateActivitiesV1>(new Uri("queue:nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-create-activities-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Aliases.Eventing.Messages.Aliases.AliasCreatedLearningModeScheduleV1>(new Uri("queue:nullbox.fabric.aliases.eventing.messages.aliases.alias-created-learning-mode-schedule-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries.DeliveryActionDecisionRollupsV1>(new Uri("queue:nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-decision-rollups-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries.DeliveryActionDecisionStatisticsV1>(new Uri("queue:nullbox.fabric.deliveries.eventing.messages.deliveries.delivery-action-decision-statistics-v1"));
        EndpointConvention.Map<Nullbox.Fabric.Aliases.Eventing.Messages.Aliases.CreateAliasMapV1>(new Uri("queue:nullbox.fabric.aliases.eventing.messages.aliases.create-alias-map-v1"));
    }

}