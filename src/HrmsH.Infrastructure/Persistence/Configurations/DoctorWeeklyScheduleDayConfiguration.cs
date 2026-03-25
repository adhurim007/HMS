using HrmsH.Domain.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class DoctorWeeklyScheduleDayConfiguration
    : IEntityTypeConfiguration<DoctorWeeklyScheduleDay>
{
    public void Configure(EntityTypeBuilder<DoctorWeeklyScheduleDay> builder)
    {
        builder.ToTable("DoctorWeeklyScheduleDays");

        builder.HasIndex(x => new { x.StaffMemberId, x.DayOfWeek }).IsUnique();

        builder.Property(x => x.DayOfWeek).IsRequired();

        builder.Property(x => x.StartTime).IsRequired();
        builder.Property(x => x.EndTime).IsRequired();

        builder.HasOne(x => x.StaffMember)
            .WithMany()
            .HasForeignKey(x => x.StaffMemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

