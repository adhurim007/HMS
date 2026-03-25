namespace HrmsH.Domain.Identity;

public static class SystemRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string HospitalAdmin = "HospitalAdmin";
    public const string Reception = "Reception";
    public const string Doctor = "Doctor";
    public const string Nurse = "Nurse";
    public const string Pharmacist = "Pharmacist";
    public const string Finance = "Finance";
    public const string Manager = "Manager";

    public static readonly IReadOnlyList<string> All = new[]
    {
        SuperAdmin,
        HospitalAdmin,
        Reception,
        Doctor,
        Nurse,
        Pharmacist,
        Finance,
        Manager
    };
}

