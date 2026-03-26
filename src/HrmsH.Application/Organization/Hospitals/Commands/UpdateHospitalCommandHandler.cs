using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Organization.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Hospitals.Commands;

public sealed class UpdateHospitalCommandHandler : IRequestHandler<UpdateHospitalCommand, HospitalDto>
{
    private readonly IHrmsDbContext _db;

    public UpdateHospitalCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<HospitalDto> Handle(UpdateHospitalCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Hospitals
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Hospital not found.");

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var exists = await _db.Hospitals
                .AnyAsync(x => x.Id != request.Id && x.Code == request.Code, cancellationToken);
            if (exists)
                throw new InvalidOperationException("Hospital code already exists.");
        }

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Address = request.Address;
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
