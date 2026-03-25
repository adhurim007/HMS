using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Departments.Commands;

public sealed class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand>
{
    private readonly IHrmsDbContext _db;

    public DeleteDepartmentCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Departments
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Department not found.");

        entity.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}

