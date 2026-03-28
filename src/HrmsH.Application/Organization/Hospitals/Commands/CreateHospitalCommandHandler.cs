using HrmsH.Application.Abstractions;
using HrmsH.Application.Organization.Dtos;
using HrmsH.Domain.Organization;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Hospitals.Commands;

public sealed class CreateHospitalCommandHandler : IRequestHandler<CreateHospitalCommand, HospitalDto>
{
    private readonly IHrmsDbContext _db;

    public CreateHospitalCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<HospitalDto> Handle(CreateHospitalCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var exists = await _db.Hospitals
                .AnyAsync(x => x.Code == request.Code, cancellationToken);
            if (exists)
                throw new InvalidOperationException("Hospital code already exists.");
        }

        var entity = new Hospital
        {
            Name = request.Name,
            Code = request.Code,
            Address = request.Address
        };

        _db.Hospitals.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var defaultFacilityName = $"{entity.Name} - Main Facility";
        var defaultFacilityCode = string.IsNullOrWhiteSpace(entity.Code)
            ? $"H{entity.Id}-MAIN"
            : $"{entity.Code}-MAIN";
        if (defaultFacilityCode.Length > 50)
        {
            defaultFacilityCode = defaultFacilityCode[..50];
        }

        var codeExists = await _db.Facilities
            .AsNoTracking()
            .AnyAsync(x => x.Code == defaultFacilityCode, cancellationToken);
        if (codeExists)
        {
            defaultFacilityCode = $"H{entity.Id}-MAIN";
        }

        var defaultFacility = new Facility
        {
            HospitalId = entity.Id,
            Name = defaultFacilityName.Length > 200 ? defaultFacilityName[..200] : defaultFacilityName,
            Code = defaultFacilityCode,
            Address = entity.Address,
            ParentId = null
        };

        _db.Facilities.Add(defaultFacility);
        await _db.SaveChangesAsync(cancellationToken);

        return new HospitalDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Address = entity.Address
        };
    }
}
