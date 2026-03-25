using HrmsH.Application.Abstractions;
using HrmsH.Application.Patients.Dtos;
using HrmsH.Domain.Patients;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Commands;

public sealed class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientDto>
{
    private readonly IHrmsDbContext _db;

    public CreatePatientCommandHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<PatientDto> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Patients.AnyAsync(
            x => x.MedicalRecordNumber == request.MedicalRecordNumber,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException("Medical record number already exists.");

        var patient = new Patient
        {
            MedicalRecordNumber = request.MedicalRecordNumber,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            BloodGroup = request.BloodGroup,
            ChronicConditions = request.ChronicConditions,
            Allergies = request.Allergies
        };

        _db.Patients.Add(patient);
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

