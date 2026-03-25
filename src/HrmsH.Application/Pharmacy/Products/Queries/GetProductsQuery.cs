using HrmsH.Application.Common.Models;
using MediatR;

namespace HrmsH.Application.Pharmacy.Products.Queries;

public sealed record GetProductsQuery(
    string? Search,
    bool? IsActive,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "Name",
    bool SortDescending = false) : IRequest<PagedResult<ProductListDto>>;

public sealed class ProductListDto
{
    public int Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? GenericName { get; init; }
    public string? Unit { get; init; }
    public decimal? DefaultSalePrice { get; init; }
    public bool IsActive { get; init; }
}
