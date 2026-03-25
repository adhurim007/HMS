using HrmsH.Domain.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class LaboratoryOrderConfiguration : IEntityTypeConfiguration<LaboratoryOrder>
{
    public void Configure(EntityTypeBuilder<LaboratoryOrder> builder)
    {
        builder.ToTable("LaboratoryOrders");
        builder.Property(x => x.ClinicalIndication).HasMaxLength(2000);
        builder.Property(x => x.PaymentMethod).HasMaxLength(100);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
    }
}

public sealed class LaboratoryOrderItemConfiguration : IEntityTypeConfiguration<LaboratoryOrderItem>
{
    public void Configure(EntityTypeBuilder<LaboratoryOrderItem> builder)
    {
        builder.ToTable("LaboratoryOrderItems");
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.IsBilled).HasDefaultValue(false);

        builder.HasOne(x => x.LaboratoryOrder)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.LaboratoryOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DiagnosticTest)
            .WithMany()
            .HasForeignKey(x => x.DiagnosticTestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class LaboratorySampleConfiguration : IEntityTypeConfiguration<LaboratorySample>
{
    public void Configure(EntityTypeBuilder<LaboratorySample> builder)
    {
        builder.ToTable("LaboratorySamples");
        builder.Property(x => x.SampleType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SampleBarcode).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.SampleBarcode).IsUnique();

        builder.HasOne(x => x.LaboratoryOrder)
            .WithMany(x => x.Samples)
            .HasForeignKey(x => x.LaboratoryOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class LaboratoryResultConfiguration : IEntityTypeConfiguration<LaboratoryResult>
{
    public void Configure(EntityTypeBuilder<LaboratoryResult> builder)
    {
        builder.ToTable("LaboratoryResults");
        builder.Property(x => x.Value).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Unit).HasMaxLength(50);
        builder.Property(x => x.ReferenceRange).HasMaxLength(100);

        builder.HasOne(x => x.LaboratoryOrderItem)
            .WithMany(x => x.Results)
            .HasForeignKey(x => x.LaboratoryOrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LaboratorySample)
            .WithMany(x => x.Results)
            .HasForeignKey(x => x.LaboratorySampleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

