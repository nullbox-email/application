using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;
using Nullbox.Fabric.Aliases.Eventing.Messages.Aliases;
using Nullbox.Fabric.Application.Common.Behaviours;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Application.Common.Validation;
using Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Accounts;
using Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Aliases;
using Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Mailboxes;
using Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Statistics;
using Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries;
using Nullbox.Fabric.Mailboxes.Eventing.Messages.Mailboxes;
using Nullbox.Security.Users.Eventing.Messages.Users;

[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Nullbox.Fabric.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(MessageBusPublishBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
        });
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IValidatorProvider, ValidatorProvider>();
        services.AddTransient<IValidationService, ValidationService>();
        services.AddTransient<IIntegrationEventHandler<AccountCreateAccountUserMapV1>, AccountCreateAccountUserMapV1Handler>();
        services.AddTransient<IIntegrationEventHandler<AccountCreateDefaultUserMailboxV1>, AccountCreateDefaultUserMailboxV1Handler>();
        services.AddTransient<IIntegrationEventHandler<AccountCreateEnablementV1>, AccountCreateEnablementV1Handler>();
        services.AddTransient<IIntegrationEventHandler<EnablementGrantCreatedV1Event>, EnablementGrantCreatedV1Handler>();
        services.AddTransient<IIntegrationEventHandler<UserProfileActivatedV1Event>, UserProfileActivatedV1Handler>();
        services.AddTransient<IIntegrationEventHandler<AliasCreatedLearningModeScheduleV1>, AliasCreatedLearningModeScheduleV1Handler>();
        services.AddTransient<IIntegrationEventHandler<AliasCreatedV1Event>, AliasCreatedV1Handler>();
        services.AddTransient<IIntegrationEventHandler<CreateAliasMapV1>, CreateAliasMapV1Handler>();
        services.AddTransient<IIntegrationEventHandler<AliasUpdatedV1Event>, UpdateAliasMapHandler>();
        services.AddTransient<IIntegrationEventHandler<AccountCreatedV1Event>, AccountCreatedV1Handler>();
        services.AddTransient<IIntegrationEventHandler<MailboxUpdateAliasMapV1>, MailboxUpdateAliasMapV1Handler>();
        services.AddTransient<IIntegrationEventHandler<MailboxUpdatedV1Event>, MailboxUpdatedV1Handler>();
        services.AddTransient<IIntegrationEventHandler<MailboxUpdateMailboxMapV1>, MailboxUpdateMailboxMapV1Handler>();
        services.AddTransient<IIntegrationEventHandler<DeliveryActionCompleteActivitiesV1>, DeliveryActionCompleteActivitiesV1Handler>();
        services.AddTransient<IIntegrationEventHandler<DeliveryActionCompletedV1Event>, DeliveryActionCompletedV1Handler>();
        services.AddTransient<IIntegrationEventHandler<DeliveryActionCompleteRollupsV1>, DeliveryActionCompleteRollupsV1Handler>();
        services.AddTransient<IIntegrationEventHandler<DeliveryActionCompleteStatisticsV1>, DeliveryActionCompleteStatisticsV1Handler>();
        return services;
    }
}