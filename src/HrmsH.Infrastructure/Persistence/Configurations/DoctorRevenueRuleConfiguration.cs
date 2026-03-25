using HrmsH.Domain.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class DoctorRevenueRuleConfiguration : IEntityTypeConfiguration<DoctorRevenueRule>
{
    public void Configure(EntityTypeBuilder<DoctorRevenueRule> builder)
    {
        builder.ToTable("DoctorRevenueRules");

        builder.Property(x => x.DoctorSharePercent)
            .HasPrecision(5, 2);

        builder.Property(x => x.HospitalSharePercent)
            .HasPrecision(5, 2);

        builder.Property(x => x.MinVisitsPerDay)
            .IsRequired();

        builder.HasOne(x => x.Doctor)
            .WithMany()
            .HasForeignKey(x => x.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ServiceItem)
            .WithMany()
            .HasForeignKey(x => x.ServiceItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

