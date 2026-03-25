using HrmsH.Application.Abstractions;
using HrmsH.Domain.Pharmacy;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Pharmacy.Stock.StockAllocation;

public sealed record FefoAllocationChunk(int StockBatchId, int ProductId, int Quantity, decimal UnitCost);

public sealed class FefoAllocationResult
{
    public required IReadOnlyList<FefoAllocationChunk> Chunks { get; init; }
    public required int AllocatedQuantity { get; init; }
    public required decimal TotalCost { get; init; }
}

/// <summary>
/// FEFO allocator: split required quantity across multiple batches ordered by expiry (oldest first).
/// </summary>
public sealed class FefoStockAllocator
{
    private readonly IHrmsDbContext _db;

    public FefoStockAllocator(IHrmsDbContext db) => _db = db;

    public async Task<FefoAllocationResult> AllocateForSale(
        int productId,
        int requiredQuantity,
        CancellationToken cancellationToken)
    {
        if (requiredQuantity <= 0)
            throw new InvalidOperationException("Required quantity must be greater than 0.");

        var productName = await _db.Products
            .AsNoTracking()
            .Where(p => p.Id == productId)
            .Select(p => p.Name)
            .FirstOrDefaultAsync(cancellationToken);

        var thresholdDate = DateTime.UtcNow; // for deterministic error message timing only

        var batches = await _db.StockBatches
            .AsNoTracking()
            .Where(b => b.ProductId == productId && b.QuantityOnHand > 0)
            .OrderBy(b => b.ExpiryDate ?? DateTime.MaxValue)
            .Select(b => new
            {
                b.Id,
                b.QuantityOnHand,
                b.UnitCost,
            })
            .ToListAsync(cancellationToken);

        var remaining = requiredQuantity;
        var chunks = new List<FefoAllocationChunk>();
        decimal totalCost = 0m;

        foreach (var batch in batches)
        {
            if (remaining == 0)
                break;

            if (!batch.UnitCost.HasValue)
                throw new InvalidOperationException(
                    $"Missing UnitCost for batch {batch.Id}. Cannot compute COGS at {thresholdDate:O}.");

            var take = Math.Min(remaining, batch.QuantityOnHand);
            if (take <= 0)
                continue;

            remaining -= take;

            var chunkUnitCost = batch.UnitCost.Value;
            chunks.Add(new FefoAllocationChunk(
                StockBatchId: batch.Id,
                ProductId: productId,
                Quantity: take,
                UnitCost: chunkUnitCost));

            totalCost += chunkUnitCost * take;
        }

        if (remaining > 0)
        {
            var name = !string.IsNullOrWhiteSpace(productName) ? productName : $"Product {productId}";
            throw new InvalidOperationException($"Insufficient stock for product '{name}'.");
        }

        return new FefoAllocationResult
        {
            Chunks = chunks,
            AllocatedQuantity = requiredQuantity,
            TotalCost = totalCost
        };
    }
}

