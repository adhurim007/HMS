using HrmsH.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Reports.Queries;

public sealed class DailyPaymentsReportQueryHandler : IRequestHandler<DailyPaymentsReportQuery, IReadOnlyList<DailyPaymentRowDto>>
{
    private readonly IHrmsDbContext _db;

    public DailyPaymentsReportQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<DailyPaymentRowDto>> Handle(DailyPaymentsReportQuery request, CancellationToken cancellationToken)
    {
        var fromDate = request.From.Date;
        var toDate = request.To.Date;

        var rows = await _db.Payments
            .AsNoTracking()
            .Where(x => x.PaymentDate >= fromDate && x.PaymentDate < toDate.AddDays(1))
            .GroupBy(x => x.PaymentDate.Date)
            .Select(g => new DailyPaymentRowDto
            {
                Date = g.Key,
                TotalAmount = g.Sum(x => x.Amount),
                PaymentCount = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        return rows;
    }
}
