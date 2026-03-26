using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Organization.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Hospitals.Queries;

public sealed class GetHospitalByIdQueryHandler : IRequestHandler<GetHospitalByIdQuery, HospitalDto>
{
    private readonly IHrmsDbContext _db;

    public GetHospitalByIdQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<HospitalDto> Handle(GetHospitalByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Hospitals
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Hospital not found.");

        return new HospitalDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Address = entity.Address
        };
    }
}
