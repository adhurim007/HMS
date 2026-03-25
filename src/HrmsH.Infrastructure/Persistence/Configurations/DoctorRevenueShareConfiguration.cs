using HrmsH.Domain.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class DoctorRevenueShareConfiguration : IEntityTypeConfiguration<DoctorRevenueShare>
{
    public void Configure(EntityTypeBuilder<DoctorRevenueShare> builder)
    {
        builder.ToTable("DoctorRevenueShares");

        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.DoctorAmount).HasPrecision(18, 2);
        builder.Property(x => x.HospitalAmount).HasPrecision(18, 2);

        builder.HasOne(x => x.Doctor)
            .WithMany()
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Invoice)
            .WithMany()
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

