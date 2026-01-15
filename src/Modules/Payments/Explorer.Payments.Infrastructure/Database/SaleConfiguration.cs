using Explorer.Payments.Core.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Explorer.Payments.Infrastructure.Database;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.DiscountPercentage)
            .IsRequired();

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate)
            .IsRequired();

        builder.Property(s => s.AuthorId)
            .IsRequired();

        // ← KLJUČNA PROMENA: Koristi Value Converter umesto Column Type
        builder.Property(s => s.TourIds)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<long>>(v, (JsonSerializerOptions)null) ?? new List<long>()
            )
            .HasColumnType("jsonb");
    }
}