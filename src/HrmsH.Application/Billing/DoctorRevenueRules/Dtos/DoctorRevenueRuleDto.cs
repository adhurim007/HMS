namespace HrmsH.Application.Billing.DoctorRevenueRules.Dtos;

public sealed record DoctorRevenueRuleDto(
    int Id,
    int? DoctorId,
    string? DoctorName,
    int? DepartmentId,
    string? DepartmentName,
    int? ServiceItemId,
    string? ServiceItemName,
    int MinVisitsPerDay,
    int? MaxVisitsPerDay,
    decimal DoctorSharePercent,
    decimal HospitalSharePercent,
    bool IsActive);

