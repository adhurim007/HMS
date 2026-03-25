namespace HrmsH.Application.Billing.Invoices.Dtos;

public sealed class UnbilledVisitServiceDto
{
    public int Id { get; init; }
    public int VisitId { get; init; }
    public DateTime VisitDate { get; init; }
    public string? DoctorName { get; init; }
    public int ServiceItemId { get; init; }
    public string ServiceName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
    public string? Notes { get; init; }
}
