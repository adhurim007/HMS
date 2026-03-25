using HrmsH.Application.Abstractions;
using HrmsH.Application.Staff.Doctors.Dtos;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Doctors.Queries;

public sealed class GetDoctorVisitSettingsByDoctorIdQueryHandler
    : IRequestHandler<GetDoctorVisitSettingsByDoctorIdQuery, DoctorVisitSettingsDto?>
{
    private readonly IHrmsDbContext _db;

    public GetDoctorVisitSettingsByDoctorIdQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<DoctorVisitSettingsDto?> Handle(
        GetDoctorVisitSettingsByDoctorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.DoctorVisitSettings
            .AsNoTracking()
            .Where(x => x.StaffMemberId == request.StaffMemberId)
            .Select(x => new DoctorVisitSettingsDto
            {
                Id = x.Id,
                StaffMemberId = x.StaffMemberId,
                MinVisitDurationMinutes = x.MinVisitDurationMinutes
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}

