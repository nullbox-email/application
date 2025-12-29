using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Deliveries;
using Nullbox.Fabric.Domain.Entities.Activities;
using Nullbox.Fabric.Domain.ValueObjects;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Activities;

public class RecentDeliveryActionConfiguration : IEntityTypeConfiguration<RecentDeliveryAction>
{
    public void Configure(EntityTypeBuilder<RecentDeliveryAction> builder)
    {
        builder.ToContainer("RecentDeliveryActions");

        builder.HasPartitionKey(x => x.PartitionKey);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .ValueGeneratedNever();

        builder.Property(x => x.PartitionKey)
            .IsRequired();

        builder.Property(x => x.DeliveryActionId)
            .IsRequired();

        builder.Property(x => x.ReceivedAt)
            .IsRequired();

        builder.Property(x => x.SortKey)
            .IsRequired();

        builder.Property(x => x.SenderDisplay)
            .IsRequired();

        builder.Property(x => x.SenderDomain)
            .IsRequired();

        builder.Property(x => x.RecipientDisplay)
            .IsRequired();

        builder.Property(x => x.RecipientDomain)
            .IsRequired();

        builder.Property(x => x.Subject)
            .IsRequired();

        builder.Property(x => x.SubjectHash)
            .IsRequired();

        builder.Property(x => x.HasAttachments)
            .IsRequired();

        builder.Property(x => x.AttachmentsCount)
            .IsRequired();

        builder.Property(x => x.Size)
            .IsRequired();

        builder.Property(x => x.DeliveryDecision)
            .IsRequired();

        builder.OwnsOne(x => x.DropReason, ConfigureDropReason);

        builder.OwnsOne(x => x.QuarantineReason, ConfigureQuarantineReason);

        builder.Property(x => x.ForwardTo);

        builder.Property(x => x.ForwardFrom);

        builder.Property(x => x.Provider);

        builder.Property(x => x.ProviderStatus);

        builder.Property(x => x.ProviderMessageId);

        builder.Property(x => x.ProviderError);

        builder.Property(x => x.CompletedAt);

        builder.Property(x => x.UpdatedAt);

        builder.ToTable(tb => tb.HasCheckConstraint("recent_delivery_action_delivery_decision_check", $"\"DeliveryDecision\" IN ({string.Join(",", Enum.GetValues<DeliveryDecision>().Select(e => $"'{e}'"))})"));

        builder.ToTable(tb => tb.HasCheckConstraint("recent_delivery_action_provider_status_check", $"\"ProviderStatus\" IS NULL OR \"ProviderStatus\" IN ({string.Join(",", Enum.GetValues<ProviderStatus>().Select(e => $"'{e}'"))})"));

        builder.Ignore(e => e.DomainEvents);
    }

    public static void ConfigureDropReason(OwnedNavigationBuilder<RecentDeliveryAction, DropReasonValueObject> builder)
    {
        builder.Property(x => x.Reason)
            .IsRequired();

        builder.Property(x => x.Message);
    }

    public static void ConfigureQuarantineReason(OwnedNavigationBuilder<RecentDeliveryAction, QuarantineReasonValueObject> builder)
    {
        builder.Property(x => x.Reason)
            .IsRequired();

        builder.Property(x => x.Message);
    }
}