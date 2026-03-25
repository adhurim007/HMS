using HrmsH.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.Property(x => x.EntityType).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(50);
        builder.Property(x => x.UserName).HasMaxLength(256);
        builder.Property(x => x.Description).HasMaxLength(2000);

        builder.HasIndex(x => x.EntityType);
        builder.HasIndex(x => x.EntityId);
        builder.HasIndex(x => x.PatientId);
        builder.HasIndex(x => x.CreatedAt);
    }
}

