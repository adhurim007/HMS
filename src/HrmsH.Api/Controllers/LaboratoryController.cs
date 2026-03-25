using HrmsH.Api.Models;
using HrmsH.Application.Abstractions;
using HrmsH.Domain.Diagnostics;
using HrmsH.Domain.Staff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Doctor,Nurse,Manager,Laboratory,Laboratori,laboratory,laboratori")]
public sealed class LaboratoryController : ControllerBase
{
    private readonly IHrmsDbContext _db;

    public LaboratoryController(IHrmsDbContext db)
    {
        _db = db;
    }

    public sealed class LaboratoryOrderItemDto
    {
        public int Id { get; init; }
        public int DiagnosticTestId { get; init; }
        public string TestName { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string? Notes { get; init; }
    }

    public sealed class LaboratoryCollectorDto
    {
        public int StaffMemberId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public StaffType StaffType { get; init; }
    }

    [HttpGet("collectors")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<LaboratoryCollectorDto>>>> GetCollectors()
    {
        var collectors = await _db.StaffMembers
            .AsNoTracking()
            .Where(x => x.IsActive && (
                x.StaffType == StaffType.Doctor ||
                x.StaffType == StaffType.Nurse ||
                x.StaffType == StaffType.Other))
            .OrderBy(x => x.FullName)
            .Select(x => new LaboratoryCollectorDto
            {
                StaffMemberId = x.Id,
                FullName = x.FullName,
                StaffType = x.StaffType
            })
            .ToListAsync();

        return Ok(ApiResponse<IReadOnlyList<LaboratoryCollectorDto>>.Ok(collectors));
    }

    public sealed class LaboratorySampleDto
    {
        public int Id { get; init; }
        public string SampleType { get; init; } = string.Empty;
        public DateTime CollectedAt { get; init; }
        public int CollectedById { get; init; }
        public string SampleBarcode { get; init; } = string.Empty;
    }

    public sealed class LaboratoryResultDto
    {
        public int Id { get; init; }
        public int LaboratoryOrderItemId { get; init; }
        public int LaboratorySampleId { get; init; }
        public string Value { get; init; } = string.Empty;
        public string? Unit { get; init; }
        public string? ReferenceRange { get; init; }
        public LaboratoryResultFlag Flag { get; init; }
        public int EnteredById { get; init; }
        public DateTime EnteredAt { get; init; }
    }

    public sealed class LaboratoryOrderDto
    {
        public int Id { get; init; }
        public int PatientId { get; init; }
        public int? VisitId { get; init; }
        public int? ReferringDoctorId { get; init; }
        public DateTime OrderedAt { get; init; }
        public LabPriority Priority { get; init; }
        public string? ClinicalIndication { get; init; }
        public decimal TotalAmount { get; init; }
        public bool IsPaid { get; init; }
        public DateTime? PaidAt { get; init; }
        public string? PaymentMethod { get; init; }
        public LaboratoryOrderStatus Status { get; init; }
        public int? ValidatedById { get; init; }
        public DateTime? ValidatedAt { get; init; }
        public DateTime? DeliveredAt { get; init; }
        public IReadOnlyList<LaboratoryOrderItemDto> Items { get; init; } = [];
        public IReadOnlyList<LaboratorySampleDto> Samples { get; init; } = [];
        public IReadOnlyList<LaboratoryResultDto> Results { get; init; } = [];
    }

    private static LaboratoryOrderDto ToDto(LaboratoryOrder x)
    {
        var results = x.Items
            .SelectMany(i => i.Results)
            .OrderBy(r => r.Id)
            .Select(r => new LaboratoryResultDto
            {
                Id = r.Id,
                LaboratoryOrderItemId = r.LaboratoryOrderItemId,
                LaboratorySampleId = r.LaboratorySampleId,
                Value = r.Value,
                Unit = r.Unit,
                ReferenceRange = r.ReferenceRange,
                Flag = r.Flag,
                EnteredById = r.EnteredById,
                EnteredAt = r.EnteredAt
            })
            .ToList();

        return new LaboratoryOrderDto
        {
            Id = x.Id,
            PatientId = x.PatientId,
            VisitId = x.VisitId,
            ReferringDoctorId = x.ReferringDoctorId,
            OrderedAt = x.OrderedAt,
            Priority = x.Priority,
            ClinicalIndication = x.ClinicalIndication,
            TotalAmount = x.TotalAmount,
            IsPaid = x.IsPaid,
            PaidAt = x.PaidAt,
            PaymentMethod = x.PaymentMethod,
            Status = x.Status,
            ValidatedById = x.ValidatedById,
            ValidatedAt = x.ValidatedAt,
            DeliveredAt = x.DeliveredAt,
            Items = x.Items.Select(i => new LaboratoryOrderItemDto
            {
                Id = i.Id,
                DiagnosticTestId = i.DiagnosticTestId,
                TestName = i.DiagnosticTest.Name,
                Price = i.Price,
                Notes = i.Notes
            }).ToList(),
            Samples = x.Samples.Select(s => new LaboratorySampleDto
            {
                Id = s.Id,
                SampleType = s.SampleType,
                CollectedAt = s.CollectedAt,
                CollectedById = s.CollectedById,
                SampleBarcode = s.SampleBarcode
            }).ToList(),
            Results = results
        };
    }

    private async Task<LaboratoryOrder?> LoadOrder(int orderId, CancellationToken ct = default)
    {
        return await _db.LaboratoryOrders
            .Include(x => x.Items).ThenInclude(i => i.DiagnosticTest)
            .Include(x => x.Items).ThenInclude(i => i.Results)
            .Include(x => x.Samples)
            .FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDeleted, ct);
    }

    private static bool IsCancelledOrDelivered(LaboratoryOrderStatus status)
        => status is LaboratoryOrderStatus.Cancelled or LaboratoryOrderStatus.Delivered;

    public sealed class CreateLaboratoryOrderRequest
    {
        public int PatientId { get; set; }
        public int? VisitId { get; set; }
        public int? ReferringDoctorId { get; set; }
        public string? ClinicalIndication { get; set; }
        public LabPriority Priority { get; set; } = LabPriority.Normal;
        public List<CreateLaboratoryOrderItemRequest> Items { get; set; } = [];
    }

    public sealed class CreateLaboratoryOrderItemRequest
    {
        public int DiagnosticTestId { get; set; }
        public string? Notes { get; set; }
    }

    [HttpPost("orders")]
    [Authorize(Roles = "Doctor,Reception,Laboratory,Laboratori,SuperAdmin,HospitalAdmin")]
    public async Task<ActionResult<ApiResponse<LaboratoryOrderDto>>> CreateOrder([FromBody] CreateLaboratoryOrderRequest request)
    {
        if (request.Items.Count == 0)
            return BadRequest(ApiResponse<LaboratoryOrderDto>.Fail("At least one laboratory test is required."));

        var distinctIds = request.Items.Select(i => i.DiagnosticTestId).Distinct().ToList();
        var tests = await _db.DiagnosticTests
            .Where(t => distinctIds.Contains(t.Id) && t.Type == DiagnosticType.Lab && t.IsActive)
            .ToDictionaryAsync(t => t.Id);

        if (tests.Count != distinctIds.Count)
            return BadRequest(ApiResponse<LaboratoryOrderDto>.Fail("One or more selected tests are invalid/inactive or not laboratory tests."));

        var order = new LaboratoryOrder
        {
            PatientId = request.PatientId,
            VisitId = request.VisitId,
            ReferringDoctorId = request.ReferringDoctorId,
            Priority = request.Priority,
            ClinicalIndication = request.ClinicalIndication?.Trim(),
            OrderedAt = DateTime.UtcNow,
            Status = LaboratoryOrderStatus.Ordered
        };

        foreach (var item in request.Items)
        {
            var test = tests[item.DiagnosticTestId];
            order.Items.Add(new LaboratoryOrderItem
            {
                DiagnosticTestId = test.Id,
                Price = test.Price,
                Notes = item.Notes?.Trim()
            });
        }
        order.TotalAmount = order.Items.Sum(i => i.Price);

        _db.LaboratoryOrders.Add(order);
        await _db.SaveChangesAsync();

        var reloaded = await LoadOrder(order.Id);
        return Ok(ApiResponse<LaboratoryOrderDto>.Ok(ToDto(reloaded!)));
    }

    [HttpGet("orders")]
    public async Task<ActionResult<PagedApiResponse<LaboratoryOrderDto>>> GetOrders(
        [FromQuery] int? patientId = null,
        [FromQuery] int? visitId = null,
        [FromQuery] int? doctorId = null,
        [FromQuery] LaboratoryOrderStatus? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var q = _db.LaboratoryOrders.AsNoTracking().AsQueryable();
        if (patientId.HasValue) q = q.Where(x => x.PatientId == patientId.Value);
        if (visitId.HasValue) q = q.Where(x => x.VisitId == visitId.Value);
        if (doctorId.HasValue) q = q.Where(x => x.ReferringDoctorId == doctorId.Value);
        if (status.HasValue) q = q.Where(x => x.Status == status.Value);
        if (from.HasValue) q = q.Where(x => x.OrderedAt >= from.Value);
        if (to.HasValue) q = q.Where(x => x.OrderedAt <= to.Value);

        var total = await q.CountAsync();
        var ids = await q.OrderByDescending(x => x.OrderedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => x.Id)
            .ToListAsync();

        var orders = await _db.LaboratoryOrders
            .Include(x => x.Items).ThenInclude(i => i.DiagnosticTest)
            .Include(x => x.Items).ThenInclude(i => i.Results)
            .Include(x => x.Samples)
            .Where(x => ids.Contains(x.Id))
            .OrderByDescending(x => x.OrderedAt)
            .ToListAsync();

        return Ok(new PagedApiResponse<LaboratoryOrderDto>
        {
            Success = true,
            TotalCount = total,
            Items = orders.Select(ToDto).ToList()
        });
    }

    public sealed class MarkOrderPaidRequest
    {
        public string? PaymentMethod { get; set; }
    }

    [HttpPut("orders/{orderId:int}/mark-paid")]
    [Authorize(Roles = "Reception,Laboratory,Laboratori,SuperAdmin,HospitalAdmin")]
    public async Task<ActionResult<ApiResponse<object>>> MarkPaid([FromRoute] int orderId, [FromBody] MarkOrderPaidRequest request)
    {
        var order = await _db.LaboratoryOrders.FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDeleted);
        if (order is null) return NotFound(ApiResponse<object>.Fail("Laboratory order not found."));
        if (IsCancelledOrDelivered(order.Status)) return BadRequest(ApiResponse<object>.Fail("Cannot mark payment on this order status."));

        order.IsPaid = true;
        order.PaidAt = DateTime.UtcNow;
        order.PaymentMethod = request.PaymentMethod?.Trim();
        if (order.Status == LaboratoryOrderStatus.Ordered) order.Status = LaboratoryOrderStatus.Paid;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    public sealed class CreateSampleRequest
    {
        public string SampleType { get; set; } = string.Empty;
        public DateTime? CollectedAt { get; set; }
        public int CollectedById { get; set; }
        public string SampleBarcode { get; set; } = string.Empty;
    }

    [HttpPost("orders/{orderId:int}/samples")]
    [Authorize(Roles = "Laboratory,Laboratori,Nurse,SuperAdmin,HospitalAdmin")]
    public async Task<ActionResult<ApiResponse<LaboratorySampleDto>>> CreateSample([FromRoute] int orderId, [FromBody] CreateSampleRequest request)
    {
        var order = await _db.LaboratoryOrders.Include(x => x.Samples).FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDeleted);
        if (order is null) return NotFound(ApiResponse<LaboratorySampleDto>.Fail("Laboratory order not found."));
        if (IsCancelledOrDelivered(order.Status)) return BadRequest(ApiResponse<LaboratorySampleDto>.Fail("Cannot collect sample for this order status."));

        var sample = new LaboratorySample
        {
            LaboratoryOrderId = order.Id,
            SampleType = request.SampleType.Trim(),
            CollectedAt = request.CollectedAt ?? DateTime.UtcNow,
            CollectedById = request.CollectedById,
            SampleBarcode = request.SampleBarcode.Trim()
        };
        _db.LaboratorySamples.Add(sample);

        order.Status = LaboratoryOrderStatus.SampleCollected;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<LaboratorySampleDto>.Ok(new LaboratorySampleDto
        {
            Id = sample.Id,
            SampleType = sample.SampleType,
            CollectedAt = sample.CollectedAt,
            CollectedById = sample.CollectedById,
            SampleBarcode = sample.SampleBarcode
        }));
    }

    [HttpPut("orders/{orderId:int}/start-processing")]
    [Authorize(Roles = "Laboratory,Laboratori,SuperAdmin,HospitalAdmin")]
    public async Task<ActionResult<ApiResponse<object>>> StartProcessing([FromRoute] int orderId)
    {
        var order = await _db.LaboratoryOrders.FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDeleted);
        if (order is null) return NotFound(ApiResponse<object>.Fail("Laboratory order not found."));
        if (order.Status != LaboratoryOrderStatus.SampleCollected && order.Status != LaboratoryOrderStatus.ReTest)
            return BadRequest(ApiResponse<object>.Fail("Order must be in SampleCollected or ReTest status."));

        order.Status = LaboratoryOrderStatus.InProcess;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    public sealed class AddResultRequest
    {
        public int LaboratoryOrderItemId { get; set; }
        public int LaboratorySampleId { get; set; }
        public string Value { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public string? ReferenceRange { get; set; }
        public LaboratoryResultFlag Flag { get; set; } = LaboratoryResultFlag.Normal;
        public int EnteredById { get; set; }
    }

    [HttpPost("orders/{orderId:int}/results")]
    [Authorize(Roles = "Laboratory,Laboratori,SuperAdmin,HospitalAdmin")]
    public async Task<ActionResult<ApiResponse<object>>> AddResult([FromRoute] int orderId, [FromBody] AddResultRequest request)
    {
        var order = await LoadOrder(orderId);
        if (order is null) return NotFound(ApiResponse<object>.Fail("Laboratory order not found."));
        if (order.Samples.Count == 0) return BadRequest(ApiResponse<object>.Fail("Cannot enter result without collected sample."));
        if (IsCancelledOrDelivered(order.Status)) return BadRequest(ApiResponse<object>.Fail("Cannot enter result for this order status."));

        var item = order.Items.FirstOrDefault(i => i.Id == request.LaboratoryOrderItemId);
        if (item is null) return BadRequest(ApiResponse<object>.Fail("Laboratory order item not found."));

        var sample = order.Samples.FirstOrDefault(s => s.Id == request.LaboratorySampleId);
        if (sample is null) return BadRequest(ApiResponse<object>.Fail("Laboratory sample not found for this order."));

        var existing = await _db.LaboratoryResults.FirstOrDefaultAsync(r =>
            r.LaboratoryOrderItemId == request.LaboratoryOrderItemId &&
            r.LaboratorySampleId == request.LaboratorySampleId &&
            !r.IsDeleted);

        if (existing is null)
        {
            _db.LaboratoryResults.Add(new LaboratoryResult
            {
                LaboratoryOrderItemId = item.Id,
                LaboratorySampleId = sample.Id,
                Value = request.Value.Trim(),
                Unit = request.Unit?.Trim(),
                ReferenceRange = request.ReferenceRange?.Trim(),
                Flag = request.Flag,
                EnteredById = request.EnteredById,
                EnteredAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.Value = request.Value.Trim();
            existing.Unit = request.Unit?.Trim();
            existing.ReferenceRange = request.ReferenceRange?.Trim();
            existing.Flag = request.Flag;
            existing.EnteredById = request.EnteredById;
            existing.EnteredAt = DateTime.UtcNow;
        }

        order.Status = LaboratoryOrderStatus.Completed;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    public sealed class ValidateResultsRequest
    {
        public int ValidatedById { get; set; }
    }

    [HttpPut("orders/{orderId:int}/validate")]
    [Authorize(Roles = "Laboratory,Laboratori,Manager,SuperAdmin,HospitalAdmin")]
    public async Task<ActionResult<ApiResponse<object>>> ValidateResults([FromRoute] int orderId, [FromBody] ValidateResultsRequest request)
    {
        var order = await LoadOrder(orderId);
        if (order is null) return NotFound(ApiResponse<object>.Fail("Laboratory order not found."));
        if (order.Items.Count == 0)
            return BadRequest(ApiResponse<object>.Fail("Order has no laboratory items."));

        var completedItemIds = order.Items
            .Where(i => i.Results.Any())
            .Select(i => i.Id)
            .ToHashSet();
        var missingResults = order.Items
            .Where(i => !completedItemIds.Contains(i.Id))
            .Select(i => i.DiagnosticTest.Name)
            .ToList();

        if (missingResults.Count > 0)
        {
            var missingLabel = string.Join(", ", missingResults);
            return BadRequest(ApiResponse<object>.Fail($"Cannot validate. Missing results for: {missingLabel}."));
        }

        order.ValidatedById = request.ValidatedById;
        order.ValidatedAt = DateTime.UtcNow;
        order.Status = LaboratoryOrderStatus.Validated;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpPut("orders/{orderId:int}/deliver")]
    [Authorize(Roles = "Reception,Doctor,Laboratory,Laboratori,Manager,SuperAdmin,HospitalAdmin")]
    public async Task<ActionResult<ApiResponse<object>>> Deliver([FromRoute] int orderId)
    {
        var order = await _db.LaboratoryOrders.FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDeleted);
        if (order is null) return NotFound(ApiResponse<object>.Fail("Laboratory order not found."));
        if (order.Status != LaboratoryOrderStatus.Validated)
            return BadRequest(ApiResponse<object>.Fail("Only validated orders can be delivered."));

        order.Status = LaboratoryOrderStatus.Delivered;
        order.DeliveredAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpPut("orders/{orderId:int}/cancel")]
    [Authorize(Roles = "Doctor,Reception,Manager,SuperAdmin,HospitalAdmin")]
    public async Task<ActionResult<ApiResponse<object>>> Cancel([FromRoute] int orderId)
    {
        var order = await _db.LaboratoryOrders.FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDeleted);
        if (order is null) return NotFound(ApiResponse<object>.Fail("Laboratory order not found."));
        if (order.Status != LaboratoryOrderStatus.Ordered && order.Status != LaboratoryOrderStatus.Paid)
            return BadRequest(ApiResponse<object>.Fail("Order can be cancelled only before sample collection."));

        order.Status = LaboratoryOrderStatus.Cancelled;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpPut("orders/{orderId:int}/retest")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Doctor,Nurse,Manager,Laboratory,Laboratori,laboratory,laboratori")]
    public async Task<ActionResult<ApiResponse<object>>> Retest([FromRoute] int orderId)
    {
        var order = await _db.LaboratoryOrders.FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDeleted);
        if (order is null) return NotFound(ApiResponse<object>.Fail("Laboratory order not found."));
        if (order.Status != LaboratoryOrderStatus.Completed &&
            order.Status != LaboratoryOrderStatus.Validated &&
            order.Status != LaboratoryOrderStatus.Delivered)
            return BadRequest(ApiResponse<object>.Fail("Re-test can be requested only after completed/validated/delivered."));

        order.Status = LaboratoryOrderStatus.ReTest;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    public sealed class PatientLabHistoryRowDto
    {
        public int LaboratoryOrderId { get; init; }
        public DateTime OrderedAt { get; init; }
        public string TestName { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
        public string? Unit { get; init; }
        public string? ReferenceRange { get; init; }
        public LaboratoryResultFlag Flag { get; init; }
        public LaboratoryOrderStatus Status { get; init; }
    }

    [HttpGet("patient-history/{patientId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PatientLabHistoryRowDto>>>> GetPatientHistory([FromRoute] int patientId)
    {
        var rows = await _db.LaboratoryResults
            .AsNoTracking()
            .Where(r => r.LaboratoryOrderItem.LaboratoryOrder.PatientId == patientId)
            .OrderByDescending(r => r.LaboratoryOrderItem.LaboratoryOrder.OrderedAt)
            .Select(r => new PatientLabHistoryRowDto
            {
                LaboratoryOrderId = r.LaboratoryOrderItem.LaboratoryOrderId,
                OrderedAt = r.LaboratoryOrderItem.LaboratoryOrder.OrderedAt,
                TestName = r.LaboratoryOrderItem.DiagnosticTest.Name,
                Value = r.Value,
                Unit = r.Unit,
                ReferenceRange = r.ReferenceRange,
                Flag = r.Flag,
                Status = r.LaboratoryOrderItem.LaboratoryOrder.Status
            })
            .ToListAsync();

        return Ok(ApiResponse<IReadOnlyList<PatientLabHistoryRowDto>>.Ok(rows));
    }
}

