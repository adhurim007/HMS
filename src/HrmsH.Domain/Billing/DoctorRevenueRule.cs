using HrmsH.Domain.Common;
using HrmsH.Domain.Organization;
using HrmsH.Domain.Staff;

namespace HrmsH.Domain.Billing;

public class DoctorRevenueRule : BaseEntity
{
    public int? DoctorId { get; set; }
    public StaffMember? Doctor { get; set; }

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int? ServiceItemId { get; set; }
    public ServiceItem? ServiceItem { get; set; }

    public int MinVisitsPerDay { get; set; }
    public int? MaxVisitsPerDay { get; set; }

    public decimal DoctorSharePercent { get; set; }
    public decimal HospitalSharePercent { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

