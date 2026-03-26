using HrmsH.Application.Common.Interfaces;

namespace HrmsH.Api.Services;

public sealed class FacilityContextService : IFacilityContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FacilityContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? ActiveFacilityId
    {
        get
        {
            var http = _httpContextAccessor.HttpContext;
            if (http is null)
            {
                return null;
            }

            if (http.Request.Headers.TryGetValue("X-Facility-Id", out var values) &&
                int.TryParse(values.FirstOrDefault(), out var headerId))
            {
                return headerId;
            }

            if (http.Request.Query.TryGetValue("facilityId", out var queryValues) &&
                int.TryParse(queryValues.FirstOrDefault(), out var queryId))
            {
                return queryId;
            }

            return null;
        }
    }
}
