using HrmsH.Domain.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Code).HasMaxLength(50);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.HasIndex(x => x.HospitalId);
        builder.HasIndex(x => x.ParentId);

        builder.HasOne(x => x.Hospital)
            .WithMany(x => x.Facilities)
            .HasForeignKey(x => x.HospitalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

