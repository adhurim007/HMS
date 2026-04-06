namespace HrmsH.Domain.Patients;

public static class VisitFormTemplates
{
    public const string General = "GENERAL";
    public const string Pediatrics = "PEDIATRICS";
    public const string Gynecology = "GYNECOLOGY";
    public const string Dentistry = "DENTISTRY";

    public static readonly string[] All = { General, Pediatrics, Gynecology, Dentistry };

    public static bool IsKnown(string? value) =>
        !string.IsNullOrEmpty(value) && All.Contains(value, StringComparer.Ordinal);
}
