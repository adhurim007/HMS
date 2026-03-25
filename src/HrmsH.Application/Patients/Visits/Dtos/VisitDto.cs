namespace HrmsH.Application.Patients.Visits.Dtos;

public sealed class VisitServiceDto
{
    public int Id { get; init; }
    public int ServiceItemId { get; init; }
    public string? ServiceName { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string? Notes { get; init; }
    public bool IsBilled { get; init; }
}

public sealed class VisitDto
{
    public int Id { get; init; }
    public int PatientId { get; init; }
    public int? DoctorId { get; init; }
    public bool HasPrescription { get; init; }
    public DateTime VisitDate { get; init; }
    public string? ChiefComplaint { get; init; }
    public string? Notes { get; init; }
    public string? Diagnosis { get; init; }

    public IReadOnlyList<VisitServiceDto> Services { get; init; } = Array.Empty<VisitServiceDto>();
}
