namespace HrmsH.Application.Billing.Dtos;

public sealed class ServiceItemDto
{
    public int Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
}
