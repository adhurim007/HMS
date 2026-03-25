using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Visits.Commands;

public sealed class DeleteVisitCommandHandler : IRequestHandler<DeleteVisitCommand>
{
    private readonly IHrmsDbContext _db;

    public DeleteVisitCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task Handle(DeleteVisitCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Visits.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Visit not found.");
        entity.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
