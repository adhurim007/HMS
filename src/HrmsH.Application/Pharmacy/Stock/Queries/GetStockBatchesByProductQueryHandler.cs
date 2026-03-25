using HrmsH.Application.Abstractions;
using HrmsH.Application.Pharmacy.Stock.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Pharmacy.Stock.Queries;

public sealed class GetStockBatchesByProductQueryHandler : IRequestHandler<GetStockBatchesByProductQuery, IReadOnlyList<StockBatchDto>>
{
    private readonly IHrmsDbContext _db;

    public GetStockBatchesByProductQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<StockBatchDto>> Handle(GetStockBatchesByProductQuery request, CancellationToken cancellationToken)
    {
        return await _db.StockBatches
            .AsNoTracking()
            .Where(x => x.ProductId == request.ProductId)
            .OrderBy(x => x.ExpiryDate)
            .Select(x => new StockBatchDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                BatchNumber = x.BatchNumber,
                ExpiryDate = x.ExpiryDate,
                UnitCost = x.UnitCost,
                QuantityOnHand = x.QuantityOnHand
            })
            .ToListAsync(cancellationToken);
    }
}
