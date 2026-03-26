using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Organization.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Facilities.Queries;

public sealed class GetFacilityByIdQueryHandler : IRequestHandler<GetFacilityByIdQuery, FacilityDto>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetFacilityByIdQueryHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<FacilityDto> Handle(GetFacilityByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Facilities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Facility not found.");
        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hospitalId && entity.HospitalId != hospitalId)
            throw new NotFoundException("Facility not found.");

        return new FacilityDto
        {
            Id = entity.Id,
            HospitalId = entity.HospitalId,
            Name = entity.Name,
            Code = entity.Code,
            Address = entity.Address,
            ParentId = entity.ParentId
        };
    }
}

