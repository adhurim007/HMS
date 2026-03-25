using HrmsH.Domain.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class DoctorVisitSettingsConfiguration
    : IEntityTypeConfiguration<DoctorVisitSettings>
{
    public void Configure(EntityTypeBuilder<DoctorVisitSettings> builder)
    {
        builder.ToTable("DoctorVisitSettings");

        builder.HasIndex(x => x.StaffMemberId).IsUnique();

        builder.Property(x => x.MinVisitDurationMinutes).IsRequired();

        builder.HasOne(x => x.StaffMember)
            .WithMany()
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

