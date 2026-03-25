using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Dtos;
using HrmsH.Domain.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Services.Commands;

public sealed class CreateServiceItemCommandHandler : IRequestHandler<CreateServiceItemCommand, ServiceItemDto>
{
    private readonly IHrmsDbContext _db;

    public CreateServiceItemCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<ServiceItemDto> Handle(CreateServiceItemCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.ServiceItems.AnyAsync(x => x.Code == request.Code, cancellationToken);
        if (exists)
            throw new InvalidOperationException("Service code already exists.");

        var entity = new ServiceItem
        {
            Code = request.Code,
            Name = request.Name,
            Price = request.Price,
            IsActive = true
        };
        _db.ServiceItems.Add(entity);
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
