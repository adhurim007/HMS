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

        await _db.SaveChangesAsync(cancellationToken);

        return new PatientDto
        {
            Id = patient.Id,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            FullName = patient.FullName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            Phone = patient.Phone,
            Email = patient.Email,
            Address = patient.Address,
            BloodGroup = patient.BloodGroup,
            ChronicConditions = patient.ChronicConditions,
            Allergies = patient.Allergies
        };
    }
}

