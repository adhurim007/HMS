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
            Allergies = request.Allergies,
            ParentGuardianName = request.ParentGuardianName,
            PediatricMtl = request.PediatricMtl,
            PediatricGjtl = request.PediatricGjtl,
            PediatricPkl = request.PediatricPkl,
            PriorLiveBirth = request.PriorLiveBirth,
            PriorAbortion = request.PriorAbortion
        };

        _db.Patients.Add(patient);
        await _db.SaveChangesAsync(cancellationToken);

        return PatientDto.FromEntity(patient);
    }
}
