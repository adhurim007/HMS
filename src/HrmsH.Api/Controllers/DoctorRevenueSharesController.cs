using HrmsH.Api.Models;
using HrmsH.Application.Abstractions;
using HrmsH.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{SystemRoles.SuperAdmin},{SystemRoles.HospitalAdmin},{SystemRoles.Manager},{SystemRoles.Finance},{SystemRoles.Doctor},{SystemRoles.Reception}")]
public sealed class DoctorRevenueSharesController : ControllerBase
{
    private readonly IHrmsDbContext _db;

    public DoctorRevenueSharesController(IHrmsDbContext db) => _db = db;

    public sealed record DoctorRevenueDailyListRowDto(
        DateTime Date,
        int TotalVisits,
        decimal TotalAmount,
        decimal DoctorAmount,
        decimal HospitalAmount);

    // GET api/DoctorRevenueShares/daily-list?doctorId=3&from=2026-03-03&to=2026-03-04
    [HttpGet("daily-list")]
    public async Task<ActionResult<ApiResponse<List<DoctorRevenueDailyListRowDto>>>> GetDailyList(
        [FromQuery] int doctorId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        if (doctorId <= 0)
        {
            return BadRequest(ApiResponse<List<DoctorRevenueDailyListRowDto>>.Fail("doctorId is required."));
        }

        var fromDate = from.Date;
        var toExclusive = to.Date.AddDays(1);

        // EF Core can throw InvalidOperationException for some GroupBy translations,
        // which our middleware maps to 409. Fetch raw rows then aggregate in-memory.
        var raw = await _db.DoctorRevenueShares
            .AsNoTracking()
            .Where(x => x.DoctorId == doctorId && x.Date >= fromDate && x.Date < toExclusive)
            .Select(x => new
            {
                x.Date,
                x.TotalAmount,
                x.DoctorAmount,
                x.HospitalAmount
            })
            .ToListAsync(cancellationToken);

        var rows = raw
            .GroupBy(x => x.Date.Date)
            .Select(g => new DoctorRevenueDailyListRowDto(
                g.Key,
                g.Count(),
                g.Sum(x => x.TotalAmount),
                g.Sum(x => x.DoctorAmount),
                g.Sum(x => x.HospitalAmount)))
            .OrderBy(x => x.Date)
            .ToList();

        return Ok(ApiResponse<List<DoctorRevenueDailyListRowDto>>.Ok(rows));
    }
}

