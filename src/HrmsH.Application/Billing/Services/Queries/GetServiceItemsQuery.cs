using HrmsH.Application.Common.Models;
using MediatR;

namespace HrmsH.Application.Billing.Services.Queries;

public sealed record GetServiceItemsQuery(
    string? Search,
    bool? IsActive,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "Name",
    bool SortDescending = false) : IRequest<PagedResult<ServiceItemListDto>>;

public sealed class ServiceItemListDto
{
    public int Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
}
