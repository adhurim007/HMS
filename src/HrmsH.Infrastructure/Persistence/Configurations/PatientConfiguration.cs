using HrmsH.Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.Property(x => x.MedicalRecordNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.ParentGuardianName).HasMaxLength(200);
        builder.Property(x => x.PediatricMtl).HasColumnType("decimal(9,2)");
        builder.Property(x => x.PediatricGjtl).HasColumnType("decimal(9,2)");
        builder.Property(x => x.PediatricPkl).HasColumnType("decimal(9,2)");

        builder.HasIndex(x => x.MedicalRecordNumber).IsUnique();
    }
}

public sealed class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.Property(x => x.ChiefComplaint).HasMaxLength(2000);
        builder.Property(x => x.Diagnosis).HasMaxLength(2000);
        builder.Property(x => x.VisitFormTemplate).IsRequired().HasMaxLength(32);
        builder.Property(x => x.ClinicalDataJson).HasColumnType("nvarchar(max)");

        builder.HasOne(x => x.Patient)
            .WithMany(x => x.Visits)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.FacilityId);
        builder.HasIndex(x => new { x.PatientId, x.VisitDate });
        builder.HasIndex(x => new { x.DoctorId, x.VisitDate });
        builder.HasIndex(x => x.VisitFormTemplate);
    }
}

