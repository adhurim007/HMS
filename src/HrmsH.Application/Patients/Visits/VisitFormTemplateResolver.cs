using HrmsH.Application.Abstractions;
using HrmsH.Domain.Patients;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Visits;

/// <summary>
/// Maps organization department <c>Code</c> to a persisted visit template. Unknown codes fall back to <see cref="VisitFormTemplates.General"/>.
/// </summary>
public static class VisitFormTemplateResolver
{
    public static string FromDepartmentCode(string? departmentCode)
    {
        if (string.IsNullOrWhiteSpace(departmentCode))
            return VisitFormTemplates.General;

        var c = departmentCode.Trim().ToUpperInvariant();
        return c switch
        {
            VisitFormTemplates.Pediatrics => VisitFormTemplates.Pediatrics,
            VisitFormTemplates.Gynecology => VisitFormTemplates.Gynecology,
            VisitFormTemplates.Dentistry => VisitFormTemplates.Dentistry,
            _ => VisitFormTemplates.General
        };
    }

    public static async Task<string> ResolveForDoctorAsync(
        IHrmsDbContext db,
        int? doctorId,
        CancellationToken cancellationToken)
    {
        if (doctorId is null or 0)
            return VisitFormTemplates.General;

        var code = await (
                from s in db.StaffMembers.AsNoTracking()
                join d in db.Departments.AsNoTracking() on s.DepartmentId equals d.Id into dg
                from d in dg.DefaultIfEmpty()
                where s.Id == doctorId
                select d != null ? d.Code : null)
            .FirstOrDefaultAsync(cancellationToken);

        return FromDepartmentCode(code);
    }
}
