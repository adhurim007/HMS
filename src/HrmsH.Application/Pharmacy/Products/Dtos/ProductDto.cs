namespace HrmsH.Application.Pharmacy.Products.Dtos;

public sealed class ProductDto
{
    public int Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? GenericName { get; init; }
    public string? Strength { get; init; }
    public string? Unit { get; init; }
    public decimal? DefaultSalePrice { get; init; }
    public bool IsActive { get; init; }
}
