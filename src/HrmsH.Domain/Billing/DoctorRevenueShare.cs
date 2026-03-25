using HrmsH.Domain.Common;
using HrmsH.Domain.Patients;
using HrmsH.Domain.Staff;

namespace HrmsH.Domain.Billing;

public class DoctorRevenueShare : BaseEntity
{
    public int DoctorId { get; set; }
    public StaffMember Doctor { get; set; } = default!;

    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    public int? VisitId { get; set; }
    public Visit? Visit { get; set; }

    public DateTime Date { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal DoctorAmount { get; set; }
    public decimal HospitalAmount { get; set; }
}

