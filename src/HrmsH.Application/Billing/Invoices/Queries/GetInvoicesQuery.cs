using HrmsH.Application.Common.Models;
using HrmsH.Domain.Billing;
using MediatR;

namespace HrmsH.Application.Billing.Invoices.Queries;

public sealed record GetInvoicesQuery(
    int? FacilityId,
    int? PatientId,
    InvoiceStatus? Status,
    DateTime? From,
    DateTime? To,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "InvoiceDate",
    bool SortDescending = true) : IRequest<PagedResult<InvoiceListDto>>;

public sealed class InvoiceListDto
{
    public int Id { get; init; }
    public required string InvoiceNumber { get; init; }
    public int? FacilityId { get; init; }
    public int PatientId { get; init; }
    public DateTime InvoiceDate { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public InvoiceStatus Status { get; init; }
}
