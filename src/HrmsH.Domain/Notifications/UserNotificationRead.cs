namespace HrmsH.Domain.Notifications;

/// <summary>
/// Tracks which notification alerts the user has marked as read/dismissed.
/// Notifications themselves are computed (e.g. from appointments, stock, invoices).
/// </summary>
public class UserNotificationRead
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public string NotificationKey { get; set; } = string.Empty;
    public DateTime ReadAt { get; set; }
}
