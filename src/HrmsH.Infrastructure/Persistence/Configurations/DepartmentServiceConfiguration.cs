using HrmsH.Domain.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class DepartmentServiceConfiguration : IEntityTypeConfiguration<DepartmentService>
{
    public void Configure(EntityTypeBuilder<DepartmentService> builder)
    {
        builder.ToTable("DepartmentServices");

        builder.HasIndex(x => new { x.DepartmentId, x.ServiceItemId }).IsUnique();

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ServiceItem)
            .WithMany()
            .HasForeignKey(x => x.ServiceItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

