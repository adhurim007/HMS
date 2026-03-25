using HrmsH.Domain.Pharmacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.GenericName).HasMaxLength(200);
        builder.Property(x => x.Strength).HasMaxLength(100);
        builder.Property(x => x.Unit).HasMaxLength(50);
        builder.Property(x => x.DefaultSalePrice).HasPrecision(18, 2);

        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class StockBatchConfiguration : IEntityTypeConfiguration<StockBatch>
{
    public void Configure(EntityTypeBuilder<StockBatch> builder)
    {
        builder.Property(x => x.BatchNumber).HasMaxLength(100);
        builder.HasIndex(x => x.ExpiryDate);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.Batches)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.HasIndex(x => x.MovementDate);
    }
}

