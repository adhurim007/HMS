using HrmsH.Domain.Billing;
using HrmsH.Domain.Diagnostics;
using HrmsH.Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class ServiceItemConfiguration : IEntityTypeConfiguration<ServiceItem>
{
    public void Configure(EntityTypeBuilder<ServiceItem> builder)
    {
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.Property(x => x.InvoiceNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.HasIndex(x => x.InvoiceNumber).IsUnique();
        builder.HasIndex(x => x.FacilityId);
    }
}

public sealed class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.Quantity).HasPrecision(18, 2);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);

        builder.HasOne(x => x.Invoice)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<LaboratoryOrderItem>(x => x.LaboratoryOrderItem)
            .WithMany()
            .HasForeignKey(x => x.LaboratoryOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.LaboratoryOrderItemId)
            .IsUnique()
            .HasFilter("[LaboratoryOrderItemId] IS NOT NULL");

        builder.HasOne(x => x.PrescriptionItem)
            .WithMany()
            .HasForeignKey(x => x.PrescriptionItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PrescriptionItemId)
            .IsUnique()
            .HasFilter("[PrescriptionItemId] IS NOT NULL");
    }
}

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Method).HasMaxLength(50);
        builder.Property(x => x.Reference).HasMaxLength(200);
        builder.HasIndex(x => x.FacilityId);

        builder.HasOne(x => x.Invoice)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.InstallmentItem)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.InstallmentItemId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public sealed class InstallmentPlanConfiguration : IEntityTypeConfiguration<InstallmentPlan>
{
    public void Configure(EntityTypeBuilder<InstallmentPlan> builder)
    {
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.HasIndex(x => x.FacilityId);

        builder.HasOne(x => x.Invoice)
            .WithMany(x => x.InstallmentPlans)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class InstallmentItemConfiguration : IEntityTypeConfiguration<InstallmentItem>
{
    public void Configure(EntityTypeBuilder<InstallmentItem> builder)
    {
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.HasIndex(x => x.FacilityId);

        builder.HasOne(x => x.InstallmentPlan)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.InstallmentPlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

