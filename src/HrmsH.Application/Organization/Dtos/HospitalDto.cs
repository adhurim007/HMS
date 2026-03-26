namespace HrmsH.Application.Organization.Dtos;

public sealed class HospitalDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? Code { get; init; }
    public string? Address { get; init; }
}
