namespace HrmsH.Application.Common.Models;

public sealed class PaginationParams
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public string? SortBy { get; init; }
    public bool SortDesc { get; init; }

    public string? Search { get; init; }
}

