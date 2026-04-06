using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Patients.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Queries;

public sealed class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto>
{
    private readonly IHrmsDbContext _db;

    public GetPatientByIdQueryHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<PatientDto> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _db.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (patient is null)
            throw new NotFoundException("Patient not found.");

        return PatientDto.FromEntity(patient);
    }
}
