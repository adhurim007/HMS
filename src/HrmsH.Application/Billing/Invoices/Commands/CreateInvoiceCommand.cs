using HrmsH.Application.Billing.Invoices.Dtos;
using MediatR;

namespace HrmsH.Application.Billing.Invoices.Commands;

public sealed record InvoiceLineInput(
    int? VisitServiceId,
    int? LaboratoryOrderItemId,
    int? ServiceItemId,
    int? ProductId,
    string Description,
    decimal UnitPrice,
    decimal Quantity,
    decimal? UnitCost = null,
    decimal? LineCost = null);

public sealed record CreateInvoiceCommand(
    int PatientId,
    DateTime? InvoiceDate,
    IReadOnlyList<InvoiceLineInput> Items) : IRequest<InvoiceDto>;
