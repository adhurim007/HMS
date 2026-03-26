using HrmsH.Domain.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class HospitalConfiguration : IEntityTypeConfiguration<Hospital>
{
    public void Configure(EntityTypeBuilder<Hospital> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Code).HasMaxLength(50);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
