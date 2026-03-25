using MediatR;

namespace HrmsH.Application.Reports.Queries;

public sealed record DailyPaymentsReportQuery(DateTime From, DateTime To) : IRequest<IReadOnlyList<DailyPaymentRowDto>>;

public sealed class DailyPaymentRowDto
{
    public DateTime Date { get; init; }
    public decimal TotalAmount { get; init; }
    public int PaymentCount { get; init; }
}
