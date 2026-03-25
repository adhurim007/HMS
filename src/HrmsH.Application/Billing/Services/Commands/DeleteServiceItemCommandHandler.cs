using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Services.Commands;

public sealed class DeleteServiceItemCommandHandler : IRequestHandler<DeleteServiceItemCommand>
{
    private readonly IHrmsDbContext _db;

    public DeleteServiceItemCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task Handle(DeleteServiceItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.ServiceItems.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Service item not found.");
        entity.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
