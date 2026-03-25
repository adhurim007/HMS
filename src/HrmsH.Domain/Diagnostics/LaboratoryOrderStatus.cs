namespace HrmsH.Domain.Diagnostics;

public enum LaboratoryOrderStatus
{
    Ordered = 1,
    Paid = 2,
    SampleCollected = 3,
    InProcess = 4,
    Completed = 5,
    Validated = 6,
    Delivered = 7,
    Cancelled = 8,
    ReTest = 9
}

