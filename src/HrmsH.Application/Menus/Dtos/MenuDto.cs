namespace HrmsH.Application.Menus.Dtos;

public sealed class MenuDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string MenuKey { get; init; }
    public string? Url { get; init; }
    public int? ParentId { get; init; }
    public int DisplayOrder { get; init; }
    public string? Icon { get; init; }
    public bool IsActive { get; init; }
}
