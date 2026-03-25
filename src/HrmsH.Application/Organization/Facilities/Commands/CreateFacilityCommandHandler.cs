using HrmsH.Application.Abstractions;
using HrmsH.Application.Organization.Dtos;
using HrmsH.Domain.Organization;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Facilities.Commands;

public sealed class CreateFacilityCommandHandler : IRequestHandler<CreateFacilityCommand, FacilityDto>
{
    private readonly IHrmsDbContext _db;

    public CreateFacilityCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<FacilityDto> Handle(CreateFacilityCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var exists = await _db.Facilities.AnyAsync(
                x => x.Code == request.Code, cancellationToken);

            if (exists)
                throw new InvalidOperationException("Facility code already exists.");
        }

        var entity = new Facility
        {
            Name = request.Name,
            Code = request.Code,
            Address = request.Address
        };

        _db.Facilities.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new FacilityDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Address = entity.Address
        };
    }
}

