namespace HrmsH.Application.Notifications;

public sealed class NotificationDto
{
    public required string Type { get; init; }
    public required string Key { get; init; }
    public required string Title { get; init; }
    public required string Message { get; init; }
    public string? Link { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsRead { get; init; }
}
