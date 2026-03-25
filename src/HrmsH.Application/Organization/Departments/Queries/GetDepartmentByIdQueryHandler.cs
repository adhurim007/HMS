using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Organization.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Departments.Queries;

public sealed class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto>
{
    private readonly IHrmsDbContext _db;

    public GetDepartmentByIdQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Department not found.");

        return new DepartmentDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            FacilityId = entity.FacilityId
        };
    }
}

