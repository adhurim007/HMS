namespace HrmsH.Application.Organization.Dtos;

public sealed class DepartmentDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? Code { get; init; }
    public int FacilityId { get; init; }
}

