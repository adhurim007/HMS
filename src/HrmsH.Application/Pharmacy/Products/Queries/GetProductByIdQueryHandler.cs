using HrmsH.Application.Abstractions;
using HrmsH.Application.Pharmacy.Products.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Pharmacy.Products.Queries;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IHrmsDbContext _db;

    public GetProductByIdQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return entity is null ? null : new ProductDto
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
