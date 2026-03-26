using Microsoft.AspNetCore.Identity;

namespace HrmsH.Infrastructure.Persistence;

public class ApplicationUser : IdentityUser<int>
{
    public int? HospitalId { get; set; }
}

