using HrmsH.Api.Models;
using HrmsH.Application.Abstractions;
using HrmsH.Domain.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Doctor,Nurse,Manager,Laboratory")]
public sealed class DiagnosticsController : ControllerBase
{
    private readonly IHrmsDbContext _db;

    public DiagnosticsController(IHrmsDbContext db)
    {
        _db = db;
    }

    public sealed class DiagnosticTestDto
    {
        public int Id { get; init; }
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public DiagnosticType Type { get; init; }
        public decimal Price { get; init; }
        public bool IsActive { get; init; }
    }

    [HttpGet("tests")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Doctor,Nurse,Manager,Laboratory")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DiagnosticTestDto>>>> GetTests(
        [FromQuery] DiagnosticType? type = null,
        [FromQuery] bool? isActive = true)
    {
        var q = _db.DiagnosticTests.AsNoTracking().AsQueryable();
        if (type.HasValue) q = q.Where(x => x.Type == type.Value);
        if (isActive.HasValue) q = q.Where(x => x.IsActive == isActive.Value);
        var list = await q
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Name)
            .Select(x => new DiagnosticTestDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Type = x.Type,
                Price = x.Price,
                IsActive = x.IsActive
            })
            .ToListAsync();
        return Ok(ApiResponse<IReadOnlyList<DiagnosticTestDto>>.Ok(list));
    }

    public sealed class UpsertDiagnosticTestRequest
    {
        public int? Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DiagnosticType Type { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
    }

    [HttpPost("tests")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager,Laboratory")]
    public async Task<ActionResult<ApiResponse<DiagnosticTestDto>>> UpsertTest([FromBody] UpsertDiagnosticTestRequest request)
    {
        DiagnosticTest entity;
        if (request.Id.HasValue && request.Id.Value > 0)
        {
            entity = await _db.DiagnosticTests.FirstOrDefaultAsync(x => x.Id == request.Id.Value)
                ?? throw new InvalidOperationException("Diagnostic test not found.");
        }
        else
        {
            entity = new DiagnosticTest();
            _db.DiagnosticTests.Add(entity);
        }

        entity.Code = request.Code.Trim();
        entity.Name = request.Name.Trim();
        entity.Type = request.Type;
        entity.Price = request.Price;
        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<DiagnosticTestDto>.Ok(new DiagnosticTestDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Type = entity.Type,
            Price = entity.Price,
            IsActive = entity.IsActive
        }));
    }
}
