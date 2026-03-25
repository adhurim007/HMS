using HrmsH.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Reports.Queries;

public sealed class StockExpiryAlertsReportQueryHandler : IRequestHandler<StockExpiryAlertsReportQuery, IReadOnlyList<StockExpiryAlertRowDto>>
{
    private readonly IHrmsDbContext _db;

    public StockExpiryAlertsReportQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<StockExpiryAlertRowDto>> Handle(StockExpiryAlertsReportQuery request, CancellationToken cancellationToken)
    {
        var thresholdDate = DateTime.UtcNow.Date.AddDays(request.DaysThreshold);

        var batches = await _db.StockBatches
            .AsNoTracking()
            .Include(x => x.Product)
            .Where(x => x.ExpiryDate != null && x.ExpiryDate <= thresholdDate && x.QuantityOnHand > 0)
            .OrderBy(x => x.ExpiryDate)
            .Select(x => new StockExpiryAlertRowDto
            {
                BatchId = x.Id,
                ProductId = x.ProductId,
                ProductCode = x.Product.Code,
                ProductName = x.Product.Name,
                BatchNumber = x.BatchNumber,
                ExpiryDate = x.ExpiryDate,
                QuantityOnHand = x.QuantityOnHand,
                DaysUntilExpiry = x.ExpiryDate != null ? (int)(x.ExpiryDate.Value.Date - DateTime.UtcNow.Date).TotalDays : null
            })
            .ToListAsync(cancellationToken);

        return batches;
    }
}
