using HrmsH.Api.Models;
using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Domain.Patients;
using HrmsH.Application.Billing.Invoices.Commands;
using HrmsH.Application.Pharmacy.Stock.Commands;
using HrmsH.Application.Pharmacy.Stock.StockAllocation;
using HrmsH.Domain.Pharmacy;
using HrmsH.Domain.Billing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor,Reception,Pharmacist,Manager")]
public sealed class PrescriptionsController : ControllerBase
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public PrescriptionsController(IHrmsDbContext db, ICurrentUserService currentUser, IMediator mediator)
    {
        _db = db;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public sealed class PrescriptionItemInput
    {
        public int ProductId { get; init; }
        public string? Dosage { get; init; }
        public string? Frequency { get; init; }
        public string? Duration { get; init; }
        public int Quantity { get; init; }
        public string? Instructions { get; init; }
    }

    public sealed class UpsertPrescriptionRequest
    {
        public int VisitId { get; init; }
        public string? Notes { get; init; }
        public IReadOnlyList<PrescriptionItemInput> Items { get; init; } = Array.Empty<PrescriptionItemInput>();
    }

    public sealed class PrescriptionItemDto
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public string? Dosage { get; init; }
        public string? Frequency { get; init; }
        public string? Duration { get; init; }
        public int Quantity { get; init; }
        public string? Instructions { get; init; }
    }

    public sealed class PrescriptionDto
    {
        public int Id { get; init; }
        public int VisitId { get; init; }
        public int PatientId { get; init; }
        public int? DoctorId { get; init; }
        public string? Notes { get; init; }
        public PrescriptionStatus Status { get; init; }
        public IReadOnlyList<PrescriptionItemDto> Items { get; init; } = Array.Empty<PrescriptionItemDto>();
    }

    public sealed class PrescriptionListItemDto
    {
        public int Id { get; init; }
        public int VisitId { get; init; }
        public int PatientId { get; init; }
        public string PatientName { get; init; } = string.Empty;
        public int? DoctorId { get; init; }
        public string? DoctorName { get; init; }
        public DateTime CreatedAt { get; init; }
        public PrescriptionStatus Status { get; init; }
    }

    public sealed class DispensePrescriptionRequest
    {
        public IReadOnlyList<DispenseItemInput> Items { get; init; } = Array.Empty<DispenseItemInput>();
    }

    public sealed class DispenseItemInput
    {
        public int PrescriptionItemId { get; init; }
        public int Quantity { get; init; }
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<PrescriptionListItemDto>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? patientId = null,
        [FromQuery] int? doctorId = null,
        [FromQuery] PrescriptionStatus? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? search = null)
    {
        var query = _db.Prescriptions
            .AsNoTracking()
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .AsQueryable();

        if (patientId is int pid)
            query = query.Where(p => p.PatientId == pid);

        if (doctorId is int did)
            query = query.Where(p => p.DoctorId == did);

        if (status is PrescriptionStatus st)
            query = query.Where(p => p.Status == st);

        if (from is DateTime f)
            query = query.Where(p => p.CreatedAt >= f);

        if (to is DateTime t)
            query = query.Where(p => p.CreatedAt <= t);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                p.Patient.FullName.Contains(term) ||
                p.Patient.MedicalRecordNumber.Contains(term) ||
                (p.Doctor != null && p.Doctor.FullName.Contains(term)));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PrescriptionListItemDto
            {
                Id = p.Id,
                VisitId = p.VisitId,
                PatientId = p.PatientId,
                PatientName = p.Patient.FullName,
                DoctorId = p.DoctorId,
                DoctorName = p.Doctor != null ? p.Doctor.FullName : null,
                CreatedAt = p.CreatedAt,
                Status = p.Status
            })
            .ToListAsync();

        return Ok(new PagedApiResponse<PrescriptionListItemDto>
        {
            Success = true,
            Items = items,
            TotalCount = total
        });
    }

    [HttpGet("by-visit/{visitId:int}")]
    public async Task<ActionResult<ApiResponse<PrescriptionDto?>>> GetByVisit([FromRoute] int visitId)
    {
        var entity = await _db.Prescriptions
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.VisitId == visitId);

        if (entity is null)
        {
            return Ok(ApiResponse<PrescriptionDto?>.Ok(null));
        }

        var dto = new PrescriptionDto
        {
            Id = entity.Id,
            VisitId = entity.VisitId,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            Notes = entity.Notes,
            Status = entity.Status,
            Items = entity.Items
                .OrderBy(i => i.Id)
                .Select(i => new PrescriptionItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Dosage = i.Dosage,
                    Frequency = i.Frequency,
                    Duration = i.Duration,
                    Quantity = i.Quantity,
                    Instructions = i.Instructions
                })
                .ToList()
        };

        return Ok(ApiResponse<PrescriptionDto?>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor,Manager")]
    public async Task<ActionResult<ApiResponse<PrescriptionDto>>> Upsert([FromBody] UpsertPrescriptionRequest request)
    {
        if (request.VisitId <= 0)
            return BadRequest(ApiResponse<PrescriptionDto>.Fail("VisitId is required."));

        var visit = await _db.Visits
            .Include(v => v.Patient)
            .FirstOrDefaultAsync(v => v.Id == request.VisitId);

        if (visit is null)
            return NotFound(ApiResponse<PrescriptionDto>.Fail("Visit not found."));

        // If current user is a doctor, ensure they only modify their own visits.
        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staff = await _db.StaffMembers
                .AsNoTracking()
                .Where(s => s.UserId == uid)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            if (staff != 0 && visit.DoctorId is int visitDoctorId && visitDoctorId != staff)
                return Forbid();
        }

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .AsNoTracking()
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p);

        Prescription? entity = await _db.Prescriptions
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.VisitId == request.VisitId);

        if (entity is null)
        {
            entity = new Prescription
            {
                VisitId = visit.Id,
                PatientId = visit.PatientId,
                DoctorId = visit.DoctorId,
                Notes = request.Notes,
                Status = PrescriptionStatus.Draft,
            };
            _db.Prescriptions.Add(entity);
        }
        else
        {
            entity.Notes = request.Notes;
        }

        // Replace items
        _db.PrescriptionItems.RemoveRange(entity.Items);
        entity.Items.Clear();

        foreach (var item in request.Items.Where(i => i.ProductId > 0 && i.Quantity > 0))
        {
            if (!products.TryGetValue(item.ProductId, out Product? product))
                continue;

            entity.Items.Add(new PrescriptionItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Dosage = item.Dosage,
                Frequency = item.Frequency,
                Duration = item.Duration,
                Quantity = item.Quantity,
                Instructions = item.Instructions
            });
        }

        await _db.SaveChangesAsync();

        var dto = new PrescriptionDto
        {
            Id = entity.Id,
            VisitId = entity.VisitId,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            Notes = entity.Notes,
            Status = entity.Status,
            Items = entity.Items
                .OrderBy(i => i.Id)
                .Select(i => new PrescriptionItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Dosage = i.Dosage,
                    Frequency = i.Frequency,
                    Duration = i.Duration,
                    Quantity = i.Quantity,
                    Instructions = i.Instructions
                })
                .ToList()
        };

        return Ok(ApiResponse<PrescriptionDto>.Ok(dto));
    }

    [HttpPost("{id:int}/dispense")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Pharmacist,Finance,Manager")]
    public async Task<ActionResult<ApiResponse<object>>> Dispense(
        [FromRoute] int id,
        [FromBody] DispensePrescriptionRequest request)
    {
        var prescription = await _db.Prescriptions
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prescription is null)
            return NotFound(ApiResponse<object>.Fail("Prescription not found."));

        if (prescription.Status == PrescriptionStatus.Cancelled)
            return BadRequest(ApiResponse<object>.Fail("Cannot dispense a cancelled prescription."));

        if (prescription.Status == PrescriptionStatus.Dispensed)
            return BadRequest(ApiResponse<object>.Fail("Prescription is already dispensed."));

        var quantitiesByItem = request.Items
            .Where(i => i.Quantity > 0)
            .ToDictionary(i => i.PrescriptionItemId, i => i.Quantity);

        var effectiveItems = prescription.Items
            .Where(i => quantitiesByItem.TryGetValue(i.Id, out var q) ? q > 0 : i.Quantity > 0)
            .ToList();

        if (effectiveItems.Count == 0)
            return BadRequest(ApiResponse<object>.Fail("No items to dispense."));

        // Aggregate required quantity per product (based on requested dispense quantities).
        var requiredPerProduct = effectiveItems
            .GroupBy(i => i.ProductId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(i => quantitiesByItem.TryGetValue(i.Id, out var q) ? q : i.Quantity));

        var productIds = requiredPerProduct.Keys.ToList();
        var products = await _db.Products
            .AsNoTracking()
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p);

        // Allocate stock FEFO across multiple batches and compute COGS.
        var allocator = new FefoStockAllocator(_db);
        var allocationsByProduct = new Dictionary<int, FefoAllocationResult>();

        foreach (var kvp in requiredPerProduct)
        {
            var productId = kvp.Key;
            var requiredQty = kvp.Value;

            allocationsByProduct[productId] = await allocator.AllocateForSale(
                productId,
                requiredQty,
                CancellationToken.None);
        }

        // Build invoice lines using product default sale prices + computed COGS.
        var lines = new List<InvoiceLineInput>();
        foreach (var kvp in requiredPerProduct)
        {
            var productId = kvp.Key;
            var qty = kvp.Value;

            if (!products.TryGetValue(productId, out var product))
                throw new InvalidOperationException($"Product not found: {productId}.");

            var unitPrice = product.DefaultSalePrice ?? 0m;
            var allocation = allocationsByProduct[productId];
            var lineCost = allocation.TotalCost;
            var unitCost = qty > 0 ? (lineCost / qty) : (decimal?)null;

            lines.Add(new InvoiceLineInput(
                VisitServiceId: null,
                LaboratoryOrderItemId: null,
                ServiceItemId: null,
                ProductId: productId,
                Description: product.Name,
                UnitPrice: unitPrice,
                Quantity: qty,
                UnitCost: unitCost,
                LineCost: lineCost));
        }

        if (lines.Count == 0)
            return BadRequest(ApiResponse<object>.Fail("No invoice lines to create."));

        var invoice = await _mediator.Send(new CreateInvoiceCommand(
            PatientId: prescription.PatientId,
            InvoiceDate: DateTime.UtcNow,
            Items: lines));

        // Record stock movements (sales) for dispensed medicines (per consumed batch).
        var saleReason = $"Prescription {prescription.Id} dispensed";
        foreach (var (productId, allocation) in allocationsByProduct)
        {
            foreach (var chunk in allocation.Chunks)
            {
                await _mediator.Send(new RecordStockMovementCommand(
                    ProductId: productId,
                    StockBatchId: chunk.StockBatchId,
                    Type: StockMovementType.Sale,
                    Quantity: chunk.Quantity,
                    Reason: saleReason,
                    IsIncreaseForAdjustment: false));
            }
        }

        prescription.Status = PrescriptionStatus.Dispensed;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new { invoiceId = invoice.Id }));
    }
}

