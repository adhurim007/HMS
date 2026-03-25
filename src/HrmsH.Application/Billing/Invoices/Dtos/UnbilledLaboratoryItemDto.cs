namespace HrmsH.Application.Billing.Invoices.Dtos;

public sealed class UnbilledLaboratoryItemDto
{
    public int Id { get; init; }
    public int LaboratoryOrderId { get; init; }
    public DateTime OrderedAt { get; init; }
    public string? DoctorName { get; init; }
    public string TestName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
