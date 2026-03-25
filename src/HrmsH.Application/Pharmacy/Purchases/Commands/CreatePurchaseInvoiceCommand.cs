using HrmsH.Application.Pharmacy.Purchases.Dtos;
using MediatR;

namespace HrmsH.Application.Pharmacy.Purchases.Commands;

public sealed record PharmacyPurchaseInvoiceItemInput(
    int ProductId,
    string? BatchNumber,
    DateTime ExpiryDate,
    int Quantity,
    decimal UnitPurchasePrice);

public sealed record CreatePurchaseInvoiceCommand(
    DateTime? InvoiceDate,
    string? SupplierName,
    string? SupplierReference,
    decimal PaidAmount,
    IReadOnlyList<PharmacyPurchaseInvoiceItemInput> Items) : IRequest<PharmacyPurchaseInvoiceDto>;

