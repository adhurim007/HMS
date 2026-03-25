using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Pharmacy.Stock.Dtos;
using HrmsH.Domain.Pharmacy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Pharmacy.Stock.Commands;

public sealed class RecordStockMovementCommandHandler : IRequestHandler<RecordStockMovementCommand, StockMovementDto>
{
    private readonly IHrmsDbContext _db;

    public RecordStockMovementCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<StockMovementDto> Handle(RecordStockMovementCommand request, CancellationToken cancellationToken)
    {
        var productExists = await _db.Products.AnyAsync(x => x.Id == request.ProductId, cancellationToken);
        if (!productExists)
            throw new NotFoundException("Product not found.");

        if (!request.StockBatchId.HasValue)
            throw new InvalidOperationException("StockBatchId is required.");

        var batch = await _db.StockBatches.FirstOrDefaultAsync(x => x.Id == request.StockBatchId.Value, cancellationToken);
        if (batch is null)
            throw new NotFoundException("Stock batch not found.");
        if (batch.ProductId != request.ProductId)
            throw new InvalidOperationException("Batch does not belong to the specified product.");

        var movementDate = DateTime.UtcNow;
        var effectiveQty = request.Quantity;
        if (request.Type == StockMovementType.Sale)
            effectiveQty = -request.Quantity;
        else if (request.Type == StockMovementType.Adjustment && !request.IsIncreaseForAdjustment)
            effectiveQty = -request.Quantity;

        if (effectiveQty < 0 && batch.QuantityOnHand + effectiveQty < 0)
            throw new InvalidOperationException("Insufficient quantity in batch.");

        var movement = new StockMovement
        {
            ProductId = request.ProductId,
            StockBatchId = request.StockBatchId,
            Type = request.Type,
            Quantity = request.Quantity,
            Reason = request.Reason,
            MovementDate = movementDate
        };
        _db.StockMovements.Add(movement);
        batch.QuantityOnHand += effectiveQty;
        await _db.SaveChangesAsync(cancellationToken);

        return new StockMovementDto
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            StockBatchId = movement.StockBatchId,
            Type = movement.Type,
            Quantity = movement.Quantity,
            Reason = movement.Reason,
            MovementDate = movement.MovementDate
        };
    }
}
