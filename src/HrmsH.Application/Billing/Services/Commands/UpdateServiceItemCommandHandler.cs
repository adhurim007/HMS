using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Dtos;
using HrmsH.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Services.Commands;

public sealed class UpdateServiceItemCommandHandler : IRequestHandler<UpdateServiceItemCommand, ServiceItemDto>
{
    private readonly IHrmsDbContext _db;

    public UpdateServiceItemCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<ServiceItemDto> Handle(UpdateServiceItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.ServiceItems.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Service item not found.");

        entity.Name = request.Name;
        entity.Price = request.Price;
        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);

        return new ServiceItemDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Price = entity.Price,
            IsActive = entity.IsActive
        };
    }
}
