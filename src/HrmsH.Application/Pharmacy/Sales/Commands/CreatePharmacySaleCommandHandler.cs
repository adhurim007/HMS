using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Invoices.Dtos;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Pharmacy.Sales.Commands;
using HrmsH.Application.Pharmacy.Stock.StockAllocation;
using HrmsH.Domain.Billing;
using HrmsH.Domain.Pharmacy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Pharmacy.Sales.Commands;

public sealed class CreatePharmacySaleCommandHandler
    : IRequestHandler<CreatePharmacySaleCommand, InvoiceDto>
{
    private readonly IHrmsDbContext _db;

    public CreatePharmacySaleCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<InvoiceDto> Handle(
        CreatePharmacySaleCommand request,
        CancellationToken cancellationToken)
    {
        var patientExists = await _db.Patients.AnyAsync(
            x => x.Id == request.PatientId,
            cancellationToken);

        if (!patientExists)
            throw new NotFoundException("Patient not found.");

        // Allow duplicate product lines by aggregating quantities.
        var quantitiesByProduct = request.Items
            .Where(i => i.Quantity > 0)
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        if (quantitiesByProduct.Count == 0)
            throw new InvalidOperationException("No items to sell.");

        var productIds = quantitiesByProduct.Keys.ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .AsNoTracking()
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

        foreach (var pid in productIds)
        {
            if (!products.ContainsKey(pid))
                throw new NotFoundException($"Product not found: {pid}.");
        }

        var invoice = new Invoice
        {
            InvoiceNumber = "INV-TMP-" + Guid.NewGuid().ToString("N")[..8],
            PatientId = request.PatientId,
            InvoiceDate = DateTime.UtcNow,
            TotalAmount = 0m,
            PaidAmount = 0m,
            Status = InvoiceStatus.Unpaid
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync(cancellationToken);

        invoice.InvoiceNumber = "INV-" + invoice.Id.ToString("D6");
        await _db.SaveChangesAsync(cancellationToken);

        var allocator = new FefoStockAllocator(_db);
        var allocationsByProduct = new Dictionary<int, FefoAllocationResult>();

        foreach (var (productId, qty) in quantitiesByProduct)
        {
            allocationsByProduct[productId] = await allocator.AllocateForSale(
                productId,
                qty,
                cancellationToken);
        }

        // Preload stock batches we will consume so we can decrement QuantityOnHand in one unit.
        var allBatchIds = allocationsByProduct.Values
            .SelectMany(a => a.Chunks)
            .Select(c => c.StockBatchId)
            .Distinct()
            .ToList();

        var batchesById = await _db.StockBatches
            .Where(b => allBatchIds.Contains(b.Id))
            .ToDictionaryAsync(b => b.Id, cancellationToken);

        var invoiceItems = new List<InvoiceItem>();
        var totalAmount = 0m;
        var saleReason = $"Pharmacy sale {invoice.InvoiceNumber}";

        foreach (var (productId, qty) in quantitiesByProduct)
        {
            var product = products[productId];
            var unitPrice = product.DefaultSalePrice ?? 0m;

            var allocation = allocationsByProduct[productId];
            var lineTotal = unitPrice * qty;
            var lineCost = allocation.TotalCost;
            decimal? unitCost = qty > 0 ? (decimal?)(lineCost / qty) : null;

            totalAmount += lineTotal;

            invoiceItems.Add(new InvoiceItem
            {
                InvoiceId = invoice.Id,
                ProductId = productId,
                Description = product.Name,
                UnitPrice = unitPrice,
                Quantity = qty,
                LineTotal = lineTotal,
                UnitCost = unitCost,
                LineCost = lineCost
            });
        }

        foreach (var (productId, allocation) in allocationsByProduct)
        {
            foreach (var chunk in allocation.Chunks)
            {
                var batch = batchesById[chunk.StockBatchId];

                if (batch.QuantityOnHand < chunk.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient quantity in batch {batch.Id}.");

                batch.QuantityOnHand -= chunk.Quantity;

                _db.StockMovements.Add(new StockMovement
                {
                    ProductId = productId,
                    StockBatchId = chunk.StockBatchId,
                    Type = StockMovementType.Sale,
                    Quantity = chunk.Quantity,
                    Reason = saleReason,
                    MovementDate = DateTime.UtcNow
                });
            }
        }

        invoice.TotalAmount = totalAmount;

        _db.InvoiceItems.AddRange(invoiceItems);
        await _db.SaveChangesAsync(cancellationToken);

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            PatientId = invoice.PatientId,
            InvoiceDate = invoice.InvoiceDate,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            Status = invoice.Status,
            Items = invoiceItems
                .Select(i => new InvoiceItemDto
                {
                    Id = i.Id,
                    ServiceItemId = i.ServiceItemId,
                    ProductId = i.ProductId,
                    LaboratoryOrderItemId = i.LaboratoryOrderItemId,
                    Description = i.Description,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal,
                    UnitCost = i.UnitCost,
                    LineCost = i.LineCost
                })
                .ToList()
        };
    }
}

