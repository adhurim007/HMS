using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Invoices.Dtos;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Domain.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Invoices.Commands;

public sealed class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
{
    private readonly IHrmsDbContext _db;
    private readonly IFacilityContextService _facilityContext;

    public CreateInvoiceCommandHandler(IHrmsDbContext db, IFacilityContextService facilityContext)
    {
        _db = db;
        _facilityContext = facilityContext;
    }

    public async Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var patientExists = await _db.Patients.AnyAsync(x => x.Id == request.PatientId, cancellationToken);
        if (!patientExists)
            throw new NotFoundException("Patient not found.");

        var invoiceDate = request.InvoiceDate ?? DateTime.UtcNow;
        var invoice = new Invoice
        {
            InvoiceNumber = "INV-TMP-" + Guid.NewGuid().ToString("N")[..8],
            FacilityId = request.FacilityId ?? _facilityContext.ActiveFacilityId,
            PatientId = request.PatientId,
            InvoiceDate = invoiceDate,
            TotalAmount = 0,
            PaidAmount = 0,
            Status = InvoiceStatus.Unpaid
        };
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync(cancellationToken);

        invoice.InvoiceNumber = "INV-" + invoice.Id.ToString("D6");
        decimal total = 0;
        int? visitId = null;
        int? doctorId = null;
        DateTime? visitDate = null;
        foreach (var line in request.Items)
        {
            decimal lineTotal;
            int? serviceItemId = line.ServiceItemId;
            int? laboratoryOrderItemId = line.LaboratoryOrderItemId;
            int? productId = line.ProductId;
            string description = line.Description;
            decimal unitPrice = line.UnitPrice;
            decimal quantity = line.Quantity;
            decimal? unitCost = line.UnitCost;
            decimal? lineCost = line.LineCost;

            if (line.VisitServiceId.HasValue)
            {
                var vs = await _db.VisitServices
                    .Include(x => x.Visit)
                    .FirstOrDefaultAsync(x => x.Id == line.VisitServiceId.Value, cancellationToken);
                if (vs == null || vs.IsBilled || vs.Visit.PatientId != request.PatientId)
                    throw new NotFoundException($"Unbilled visit service {line.VisitServiceId} not found or already billed.");

                visitId ??= vs.VisitId;
                doctorId ??= vs.Visit.DoctorId;
                visitDate ??= vs.Visit.VisitDate.Date;

                serviceItemId = vs.ServiceItemId;
                var serviceItem = await _db.ServiceItems.FindAsync([vs.ServiceItemId], cancellationToken);
                description = serviceItem?.Name ?? "Service";
                unitPrice = vs.UnitPrice;
                quantity = vs.Quantity;
                vs.IsBilled = true;
                unitCost = null;
                lineCost = null;
            }
            else if (line.LaboratoryOrderItemId.HasValue)
            {
                var loi = await _db.LaboratoryOrderItems
                    .Include(x => x.LaboratoryOrder)
                    .Include(x => x.DiagnosticTest)
                    .FirstOrDefaultAsync(x => x.Id == line.LaboratoryOrderItemId.Value, cancellationToken);
                if (loi == null || loi.IsBilled || loi.LaboratoryOrder.PatientId != request.PatientId)
                    throw new NotFoundException($"Unbilled laboratory item {line.LaboratoryOrderItemId} not found or already billed.");

                laboratoryOrderItemId = loi.Id;
                description = loi.DiagnosticTest.Name;
                unitPrice = loi.Price;
                quantity = 1;
                loi.IsBilled = true;
                loi.BilledAt = DateTime.UtcNow;
                unitCost = null;
                lineCost = null;
            }

            lineTotal = unitPrice * quantity;
            total += lineTotal;
            _db.InvoiceItems.Add(new InvoiceItem
            {
                InvoiceId = invoice.Id,
                ServiceItemId = serviceItemId,
                ProductId = productId,
                LaboratoryOrderItemId = laboratoryOrderItemId,
                Description = description,
                UnitPrice = unitPrice,
                Quantity = quantity,
                LineTotal = lineTotal,
                UnitCost = unitCost,
                LineCost = lineCost ?? (unitCost.HasValue ? unitCost.Value * quantity : null)
            });
        }
        invoice.TotalAmount = total;
        await _db.SaveChangesAsync(cancellationToken);

        // If this invoice is for a doctor's visit, record revenue share based on configured rules.
        if (doctorId.HasValue && visitDate.HasValue)
        {
            var day = visitDate.Value.Date;

            // Count existing shares for this doctor for the same day to know which visit number this is.
            var visitsSoFar = await _db.DoctorRevenueShares
                .CountAsync(x => x.DoctorId == doctorId.Value && x.Date == day, cancellationToken);
            var currentVisitNumber = visitsSoFar + 1;

            var rule = await _db.DoctorRevenueRules
                .Where(x => x.DoctorId == doctorId.Value && x.IsActive)
                .Where(x => x.MinVisitsPerDay <= currentVisitNumber)
                .Where(x => !x.MaxVisitsPerDay.HasValue || x.MaxVisitsPerDay.Value >= currentVisitNumber)
                .OrderBy(x => x.MinVisitsPerDay)
                .FirstOrDefaultAsync(cancellationToken);

            if (rule is not null)
            {
                var doctorAmount = Math.Round(invoice.TotalAmount * rule.DoctorSharePercent / 100m, 2);
                var hospitalAmount = invoice.TotalAmount - doctorAmount;

                _db.DoctorRevenueShares.Add(new DoctorRevenueShare
                {
                    DoctorId = doctorId.Value,
                    InvoiceId = invoice.Id,
                    VisitId = visitId,
                    Date = day,
                    TotalAmount = invoice.TotalAmount,
                    DoctorAmount = doctorAmount,
                    HospitalAmount = hospitalAmount,
                });

                await _db.SaveChangesAsync(cancellationToken);
            }
        }

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
            FacilityId = invoice.FacilityId,
            PatientId = invoice.PatientId,
            InvoiceDate = invoice.InvoiceDate,
            TotalAmount = invoice.TotalAmount,
            PaidAmount = invoice.PaidAmount,
            Status = invoice.Status,
            Items = items
        };
    }
}
