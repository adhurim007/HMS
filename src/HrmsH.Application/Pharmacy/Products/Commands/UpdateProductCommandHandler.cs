using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Pharmacy.Products.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Pharmacy.Products.Commands;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IHrmsDbContext _db;

    public UpdateProductCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Products.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Product not found.");

        entity.Name = request.Name;
        entity.GenericName = request.GenericName;
        entity.Strength = request.Strength;
        entity.Unit = request.Unit;
        entity.DefaultSalePrice = request.DefaultSalePrice;
        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);

        return new ProductDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            GenericName = entity.GenericName,
            Strength = entity.Strength,
            Unit = entity.Unit,
            DefaultSalePrice = entity.DefaultSalePrice,
            IsActive = entity.IsActive
        };
    }
}
