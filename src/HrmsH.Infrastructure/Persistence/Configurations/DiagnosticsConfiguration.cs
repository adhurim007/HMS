using HrmsH.Domain.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class DiagnosticTestConfiguration : IEntityTypeConfiguration<DiagnosticTest>
{
    public void Configure(EntityTypeBuilder<DiagnosticTest> builder)
    {
        builder.ToTable("DiagnosticTests");
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
