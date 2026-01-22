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

        builder.Property(s => s.TourIds)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<long>>(v, (JsonSerializerOptions)null) ?? new List<long>()
            )
            .HasColumnType("jsonb")
            .Metadata.SetValueComparer( // Ovo dodato zbog greske prlikom pokretanja migracija
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<long>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (h, v) => HashCode.Combine(h, v.GetHashCode())),
                    c => c.ToList()
                )
            );
    }
}