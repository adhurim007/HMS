using HrmsH.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HrmsH.Infrastructure.Persistence.Configurations;

public sealed class UserNotificationReadConfiguration : IEntityTypeConfiguration<UserNotificationRead>
{
    public void Configure(EntityTypeBuilder<UserNotificationRead> builder)
    {
        builder.ToTable("UserNotificationReads");
        builder.HasIndex(x => new { x.UserId, x.NotificationType, x.NotificationKey }).IsUnique();
        builder.Property(x => x.NotificationType).IsRequired().HasMaxLength(64);
        builder.Property(x => x.NotificationKey).IsRequired().HasMaxLength(128);
    }
}
