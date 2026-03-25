namespace HrmsH.Application.Patients.Visits.Commands;

public sealed record VisitServiceInput(
    int ServiceItemId,
    int Quantity,
    decimal? UnitPrice,
    string? Notes);

