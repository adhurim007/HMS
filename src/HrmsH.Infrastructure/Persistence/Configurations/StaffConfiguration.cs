using HrmsH.Domain.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class StaffMemberConfiguration : IEntityTypeConfiguration<StaffMember>
{
    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(200);
    }
}

public sealed class StaffFacilityAssignmentConfiguration : IEntityTypeConfiguration<StaffFacilityAssignment>
{
    public void Configure(EntityTypeBuilder<StaffFacilityAssignment> builder)
    {
        builder.HasIndex(x => new { x.StaffMemberId, x.FacilityId }).IsUnique();

        builder.HasOne(x => x.StaffMember)
            .WithMany(x => x.FacilityAssignments)
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class DoctorProfileConfiguration : IEntityTypeConfiguration<DoctorProfile>
{
    public void Configure(EntityTypeBuilder<DoctorProfile> builder)
    {
        builder.Property(x => x.Specialty).HasMaxLength(200);
        builder.Property(x => x.LicenseNumber).HasMaxLength(100);

        builder.HasIndex(x => x.StaffMemberId).IsUnique();

        builder.HasOne(x => x.StaffMember)
            .WithMany()
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

