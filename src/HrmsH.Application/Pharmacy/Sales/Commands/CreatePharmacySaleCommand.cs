using HrmsH.Application.Billing.Invoices.Dtos;
using MediatR;

namespace HrmsH.Application.Pharmacy.Sales.Commands;

public sealed record PharmacySaleItemInput(
    int ProductId,
    int Quantity);

public sealed record CreatePharmacySaleCommand(
    int PatientId,
    IReadOnlyList<PharmacySaleItemInput> Items) : IRequest<InvoiceDto>;

