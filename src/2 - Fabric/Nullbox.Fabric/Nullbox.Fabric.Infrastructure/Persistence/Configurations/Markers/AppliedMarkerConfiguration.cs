using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Entities.Markers;
using Nullbox.Fabric.Domain.Markers;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Markers;

public class AppliedMarkerConfiguration : IEntityTypeConfiguration<AppliedMarker>
{
    public void Configure(EntityTypeBuilder<AppliedMarker> builder)
    {
        builder.ToContainer("AppliedMarkers");

        builder.HasPartitionKey(x => x.PartitionKey);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .ValueGeneratedNever();

        builder.Property(x => x.PartitionKey)
            .IsRequired();

        builder.Property(x => x.DeliveryActionId)
            .IsRequired();

        builder.Property(x => x.Stage)
            .IsRequired();

        builder.Property(x => x.Group)
            .IsRequired();

        builder.Property(x => x.Key)
            .IsRequired();

        builder.Property(x => x.AppliedAt)
            .IsRequired();

        builder.ToTable(tb => tb.HasCheckConstraint("applied_marker_stage_check", $"\"Stage\" IN ({string.Join(",", Enum.GetValues<MarkerStage>().Select(e => $"'{e}'"))})"));

        builder.ToTable(tb => tb.HasCheckConstraint("applied_marker_group_check", $"\"Group\" IN ({string.Join(",", Enum.GetValues<MarkerGroup>().Select(e => $"'{e}'"))})"));

        builder.Ignore(e => e.DomainEvents);
    }
}