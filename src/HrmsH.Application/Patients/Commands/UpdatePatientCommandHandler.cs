using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Patients.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Commands;

public sealed class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, PatientDto>
{
    private readonly IHrmsDbContext _db;

    public UpdatePatientCommandHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<PatientDto> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (patient is null)
            throw new NotFoundException("Patient not found.");

        patient.FullName = request.FullName;
        patient.DateOfBirth = request.DateOfBirth;
        patient.Gender = request.Gender;
        patient.Phone = request.Phone;
        patient.Email = request.Email;
        patient.Address = request.Address;
        patient.BloodGroup = request.BloodGroup;
        patient.ChronicConditions = request.ChronicConditions;
        patient.Allergies = request.Allergies;
        patient.ParentGuardianName = request.ParentGuardianName;
        patient.PediatricMtl = request.PediatricMtl;
        patient.PediatricGjtl = request.PediatricGjtl;
        patient.PediatricPkl = request.PediatricPkl;
        patient.PriorLiveBirth = request.PriorLiveBirth;
        patient.PriorAbortion = request.PriorAbortion;

        await _db.SaveChangesAsync(cancellationToken);

        return PatientDto.FromEntity(patient);
    }
}
