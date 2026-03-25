using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Pharmacy.Purchases.Dtos;
using HrmsH.Domain.Billing;
using HrmsH.Domain.Pharmacy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Pharmacy.Purchases.Commands;

public sealed class CreatePurchaseInvoiceCommandHandler
    : IRequestHandler<CreatePurchaseInvoiceCommand, PharmacyPurchaseInvoiceDto>
{
    private readonly IHrmsDbContext _db;

    public CreatePurchaseInvoiceCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<PharmacyPurchaseInvoiceDto> Handle(
        CreatePurchaseInvoiceCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Items.Count == 0)
            throw new InvalidOperationException("Purchase invoice must have at least one item.");

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();

        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

        foreach (var item in request.Items)
        {
            if (!products.ContainsKey(item.ProductId))
                throw new NotFoundException($"Product not found: {item.ProductId}.");
        }

        var invoiceDate = request.InvoiceDate ?? DateTime.UtcNow;

        var totalAmount = request.Items.Sum(i => i.UnitPurchasePrice * i.Quantity);
        var paidAmount = request.PaidAmount;

        var status = paidAmount <= 0
            ? InvoiceStatus.Unpaid
            : paidAmount < totalAmount
                ? InvoiceStatus.PartiallyPaid
                : InvoiceStatus.Paid;

        var invoice = new PharmacyPurchaseInvoice
        {
            InvoiceNumber = "PINV-TMP-" + Guid.NewGuid().ToString("N")[..8],
            InvoiceDate = invoiceDate,
            SupplierName = request.SupplierName,
            SupplierReference = request.SupplierReference,
            TotalAmount = totalAmount,
            PaidAmount = paidAmount,
            Status = status
        };

        _db.PharmacyPurchaseInvoices.Add(invoice);
        await _db.SaveChangesAsync(cancellationToken);

        invoice.InvoiceNumber = "PINV-" + invoice.Id.ToString("D6");
        await _db.SaveChangesAsync(cancellationToken);

        var items = new List<PharmacyPurchaseInvoiceItem>();
        var batchesToFill = new List<(StockBatch batch, PharmacyPurchaseInvoiceItemInput input)>();

        foreach (var line in request.Items)
        {
            var lineTotal = line.UnitPurchasePrice * line.Quantity;

            items.Add(new PharmacyPurchaseInvoiceItem
            {
                PharmacyPurchaseInvoiceId = invoice.Id,
                ProductId = line.ProductId,
                BatchNumber = line.BatchNumber,
                ExpiryDate = line.ExpiryDate,
                Quantity = line.Quantity,
                UnitPurchasePrice = line.UnitPurchasePrice,
                LineTotal = lineTotal
            });

            batchesToFill.Add((
                new StockBatch
                {
                    ProductId = line.ProductId,
                    BatchNumber = line.BatchNumber,
                    ExpiryDate = line.ExpiryDate,
                    QuantityOnHand = 0,
                    UnitCost = line.UnitPurchasePrice
                },
                line));
        }

        _db.PharmacyPurchaseInvoiceItems.AddRange(items);

        foreach (var (batch, _) in batchesToFill)
            _db.StockBatches.Add(batch);

        await _db.SaveChangesAsync(cancellationToken);

        var purchaseReason = $"Purchase invoice {invoice.InvoiceNumber}";
        foreach (var (batch, input) in batchesToFill)
        {
            _db.StockMovements.Add(new StockMovement
            {
                ProductId = input.ProductId,
                StockBatchId = batch.Id,
                Type = StockMovementType.Purchase,
                Quantity = input.Quantity,
                Reason = purchaseReason,
                MovementDate = DateTime.UtcNow
            });

            // Increase batch quantity on hand (StockMovement handler also does this,
            // but we are handling it inline to keep it in one transaction).
            batch.QuantityOnHand += input.Quantity;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new PharmacyPurchaseInvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            SupplierName = invoice.SupplierName,
            SupplierReference = invoice.SupplierReference,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            Status = invoice.Status,
            Items = items
                .Select(i => new PharmacyPurchaseInvoiceItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    BatchNumber = i.BatchNumber,
                    ExpiryDate = i.ExpiryDate,
                    Quantity = i.Quantity,
                    UnitPurchasePrice = i.UnitPurchasePrice,
                    LineTotal = i.LineTotal
                })
                .ToList()
        };
    }
}

