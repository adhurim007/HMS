using HrmsH.Api.Models;
using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Patients.Visits.Commands;
using HrmsH.Application.Patients.Visits.Dtos;
using HrmsH.Application.Patients.Visits.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Doctor,Nurse,Manager,Laboratory")]
public sealed class VisitsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly IHrmsDbContext _db;

    public VisitsController(IMediator mediator, ICurrentUserService currentUser, IHrmsDbContext db)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<VisitListDto>>> GetList(
        [FromQuery] int? facilityId = null,
        [FromQuery] int? patientId = null,
        [FromQuery] int? doctorId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = true)
    {
        if (!User.IsInRole("SuperAdmin") && User.IsInRole("Doctor") && _currentUser.UserId is int uid)
        {
            var staffId = await _db.StaffMembers
                .AsNoTracking()
                .Where(s => s.UserId == uid)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
            if (staffId != 0)
                doctorId = staffId;
        }

        var result = await _mediator.Send(new GetVisitsQuery(facilityId, patientId, doctorId, from, to, page, pageSize, sortBy, sortDescending));
        return Ok(new PagedApiResponse<VisitListDto>
        {
            Success = true,
            Items = result.Items,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<VisitDto>>> GetById([FromRoute] int id)
    {
        var dto = await _mediator.Send(new GetVisitByIdQuery(id));
        if (dto is null)
            return NotFound(ApiResponse<VisitDto>.Fail("Visit not found."));
        return Ok(ApiResponse<VisitDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<VisitDto>>> Create([FromBody] CreateVisitCommand command)
    {
        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staffId = await _db.StaffMembers
                .AsNoTracking()
                .Where(s => s.UserId == uid)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
            if (staffId != 0)
                command = command with { DoctorId = staffId };
        }
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<VisitDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<VisitDto>>> Update([FromRoute] int id, [FromBody] UpdateVisitCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<VisitDto>.Fail("Route id does not match body id."));
        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staffId = await _db.StaffMembers
                .AsNoTracking()
                .Where(s => s.UserId == uid)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
            if (staffId != 0)
            {
                var existingDoctorId = await _db.Visits
                    .AsNoTracking()
                    .Where(v => v.Id == id)
                    .Select(v => (int?)v.DoctorId)
                    .FirstOrDefaultAsync();
                if (existingDoctorId.HasValue && existingDoctorId.Value != staffId)
                    return Forbid();
                command = command with { DoctorId = staffId };
            }
        }
        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<VisitDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromRoute] int id)
    {
        await _mediator.Send(new DeleteVisitCommand(id));
        return Ok(ApiResponse<object>.Ok(new { }));
    }
}
