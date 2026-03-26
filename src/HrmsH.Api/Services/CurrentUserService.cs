using HrmsH.Application.Common.Interfaces;
using System.Security.Claims;

namespace HrmsH.Api.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(val, out var id) ? id : null;
        }
    }

    public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    public int? HospitalId
    {
        get
        {
            var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue("hospital_id");
            return int.TryParse(val, out var id) ? id : null;
        }
    }

    public bool IsSuperAdmin =>
        _httpContextAccessor.HttpContext?.User?.IsInRole("SuperAdmin") == true;

    public bool IsHospitalAdmin =>
        _httpContextAccessor.HttpContext?.User?.IsInRole("HospitalAdmin") == true;
}

