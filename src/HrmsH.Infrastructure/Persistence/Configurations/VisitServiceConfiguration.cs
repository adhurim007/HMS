using HrmsH.Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class VisitServiceConfiguration : IEntityTypeConfiguration<VisitService>
{
    public void Configure(EntityTypeBuilder<VisitService> builder)
    {
        builder.ToTable("VisitServices");

        builder.Property(x => x.Quantity).HasDefaultValue(1);
        builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Visit)
            .WithMany(x => x.VisitServices)
            .HasForeignKey(x => x.VisitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

