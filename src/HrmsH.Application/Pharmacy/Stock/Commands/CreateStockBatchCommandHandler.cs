using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Pharmacy.Stock.Dtos;
using HrmsH.Domain.Pharmacy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Pharmacy.Stock.Commands;

public sealed class CreateStockBatchCommandHandler : IRequestHandler<CreateStockBatchCommand, StockBatchDto>
{
    private readonly IHrmsDbContext _db;

    public CreateStockBatchCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<StockBatchDto> Handle(CreateStockBatchCommand request, CancellationToken cancellationToken)
    {
        var productExists = await _db.Products.AnyAsync(x => x.Id == request.ProductId, cancellationToken);
        if (!productExists)
            throw new NotFoundException("Product not found.");

        var entity = new StockBatch
        {
            ProductId = request.ProductId,
            BatchNumber = request.BatchNumber,
            ExpiryDate = request.ExpiryDate,
            QuantityOnHand = request.Quantity,
            UnitCost = request.UnitCost
        };
        _db.StockBatches.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new StockBatchDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            BatchNumber = entity.BatchNumber,
            ExpiryDate = entity.ExpiryDate,
            UnitCost = entity.UnitCost,
            QuantityOnHand = entity.QuantityOnHand
        };
    }
}
