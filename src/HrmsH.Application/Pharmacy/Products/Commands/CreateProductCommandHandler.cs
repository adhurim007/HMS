using HrmsH.Application.Abstractions;
using HrmsH.Application.Pharmacy.Products.Dtos;
using HrmsH.Domain.Pharmacy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Pharmacy.Products.Commands;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IHrmsDbContext _db;

    public CreateProductCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Products.AnyAsync(x => x.Code == request.Code, cancellationToken);
        if (exists)
            throw new InvalidOperationException("Product code already exists.");

        var entity = new Product
        {
            Code = request.Code,
            Name = request.Name,
            GenericName = request.GenericName,
            Strength = request.Strength,
            Unit = request.Unit,
            DefaultSalePrice = request.DefaultSalePrice,
            IsActive = true
        };
        _db.Products.Add(entity);
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
