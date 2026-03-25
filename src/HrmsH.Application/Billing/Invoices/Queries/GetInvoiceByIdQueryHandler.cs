using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Invoices.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Invoices.Queries;

public sealed class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    private readonly IHrmsDbContext _db;

    public GetInvoiceByIdQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _db.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (invoice is null) return null;

        var items = await _db.InvoiceItems
            .AsNoTracking()
            .Where(x => x.InvoiceId == invoice.Id)
            .Select(x => new InvoiceItemDto
            {
                Id = x.Id,
                ServiceItemId = x.ServiceItemId,
                ProductId = x.ProductId,
                LaboratoryOrderItemId = x.LaboratoryOrderItemId,
                Description = x.Description,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity,
                LineTotal = x.LineTotal,
                UnitCost = x.UnitCost,
                LineCost = x.LineCost
            })
            .ToListAsync(cancellationToken);

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            PatientId = invoice.PatientId,
            InvoiceDate = invoice.InvoiceDate,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            Status = invoice.Status,
            Items = items
        };
    }
}
