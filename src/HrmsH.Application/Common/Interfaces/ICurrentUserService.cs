namespace HrmsH.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? UserName { get; }
    int? HospitalId { get; }
    bool IsSuperAdmin { get; }
    bool IsHospitalAdmin { get; }
}

