using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Services.Queries;

public sealed class GetServiceItemByIdQueryHandler : IRequestHandler<GetServiceItemByIdQuery, ServiceItemDto?>
{
    private readonly IHrmsDbContext _db;

    public GetServiceItemByIdQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<ServiceItemDto?> Handle(GetServiceItemByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.ServiceItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return entity is null ? null : new ServiceItemDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Price = entity.Price,
            IsActive = entity.IsActive
        };
    }
}
