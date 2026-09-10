using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetRentManagerApi.Domain.Properties;

namespace NetRentManagerApi.Infrastructure.Persistence.Configurations;

public sealed class PropertyStatusConfiguration : IEntityTypeConfiguration<PropertyStatusCatalogEntry>
{
    public void Configure(EntityTypeBuilder<PropertyStatusCatalogEntry> builder)
    {
        builder.ToTable("property_statuses");

        builder.HasKey(status => status.Value);

        builder.Property(status => status.Value)
            .HasColumnName("value")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(status => status.Description)
            .HasColumnName("description")
            .HasMaxLength(300)
            .IsRequired();

        builder.HasIndex(status => status.Value)
            .IsUnique()
            .HasDatabaseName("ix_property_statuses_value");
    }
}