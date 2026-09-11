using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetRentManagerApi.Domain.Properties;

namespace NetRentManagerApi.Infrastructure.Persistence.Configurations;

public sealed class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable(
            "properties",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("ck_properties_price_non_negative", "price >= 0");
                tableBuilder.HasCheckConstraint("ck_properties_bedroom_count_non_negative", "bedroom_count >= 0");
                tableBuilder.HasCheckConstraint("ck_properties_bathroom_count_non_negative", "bathroom_count >= 0");
                tableBuilder.HasCheckConstraint("ck_properties_area_square_meters_positive", "area_square_meters > 0");
            });

        builder.HasKey(property => property.Id);

        builder.Property(property => property.Id)
            .HasColumnName("id");

        builder.Property(property => property.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(property => property.Description)
            .HasColumnName("description")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(property => property.Address)
            .HasColumnName("address")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(property => property.Price)
            .HasColumnName("price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(property => property.Status)
            .HasColumnName("status")
            .HasMaxLength(32)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(property => property.BedroomCount)
            .HasColumnName("bedroom_count")
            .IsRequired();

        builder.Property(property => property.BathroomCount)
            .HasColumnName("bathroom_count")
            .IsRequired();

        builder.Property(property => property.AreaSquareMeters)
            .HasColumnName("area_square_meters")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(property => property.ImageUrl)
            .HasColumnName("image_url")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(property => property.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(property => property.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.HasIndex(property => property.Status)
            .HasDatabaseName("ix_properties_status");
    }
}