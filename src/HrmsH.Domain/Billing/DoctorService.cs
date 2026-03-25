using HrmsH.Domain.Common;
using HrmsH.Domain.Staff;

namespace HrmsH.Domain.Billing;

public class DoctorService : BaseEntity
{
    public int StaffMemberId { get; set; }
    public StaffMember StaffMember { get; set; } = default!;

    public int ServiceItemId { get; set; }
    public ServiceItem ServiceItem { get; set; } = default!;
}

