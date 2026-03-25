using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Pharmacy.Products.Commands;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IHrmsDbContext _db;

    public DeleteProductCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Products.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Product not found.");
        entity.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
