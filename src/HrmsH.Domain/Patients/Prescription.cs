using HrmsH.Domain.Common;
using HrmsH.Domain.Pharmacy;
using HrmsH.Domain.Staff;

namespace HrmsH.Domain.Patients;

public class Prescription : BaseEntity
{
    public int VisitId { get; set; }
    public Visit Visit { get; set; } = default!;

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public int? DoctorId { get; set; }
    public StaffMember? Doctor { get; set; }

    public string? Notes { get; set; }
    public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Draft;

    public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
}

public class PrescriptionItem : BaseEntity
{
    public int PrescriptionId { get; set; }
    public Prescription Prescription { get; set; } = default!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public string ProductName { get; set; } = default!;

    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public int Quantity { get; set; }
    public string? Instructions { get; set; }

    public bool IsBilled { get; set; }
}

