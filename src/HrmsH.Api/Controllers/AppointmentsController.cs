using HrmsH.Api.Models;
using HrmsH.Application.Abstractions;
using HrmsH.Application.Appointments.Commands;
using HrmsH.Application.Appointments.Dtos;
using HrmsH.Application.Appointments.Queries;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Common.Models;
using HrmsH.Domain.Appointments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Doctor,Manager")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly IHrmsDbContext _db;

    public AppointmentsController(IMediator mediator, ICurrentUserService currentUser, IHrmsDbContext db)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<AppointmentDto>>> Get(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "date",
        [FromQuery] bool sortDesc = false,
        [FromQuery] string? search = null,
        [FromQuery] int? patientId = null,
        [FromQuery] int? doctorId = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] AppointmentStatus? status = null)
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

        var result = await _mediator.Send(new GetAppointmentsQuery(
            new PaginationParams
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDesc = sortDesc,
                Search = search
            },
            patientId,
            doctorId,
            departmentId,
            from,
            to,
            status));

        return Ok(new PagedApiResponse<AppointmentDto>
        {
            Success = true,
            Items = result.Items,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetById(int id)
    {
        var dto = await _mediator.Send(new GetAppointmentByIdQuery(id));

        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staffId = await _db.StaffMembers.AsNoTracking()
                .Where(s => s.UserId == uid)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
            if (staffId != 0 && dto.DoctorId != staffId)
                return Forbid();
        }

        return Ok(ApiResponse<AppointmentDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Create([FromBody] CreateAppointmentCommand command)
    {
        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staff = await _db.StaffMembers.AsNoTracking()
                .Where(s => s.UserId == uid)
                .Select(s => new { s.Id, s.DepartmentId })
                .FirstOrDefaultAsync();
            if (staff is null)
                return Forbid();
            command = command with { DoctorId = staff.Id, DepartmentId = staff.DepartmentId };
        }

        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<AppointmentDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Update(int id, [FromBody] UpdateAppointmentCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<AppointmentDto>.Fail("Route id does not match body id."));

        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staff = await _db.StaffMembers.AsNoTracking()
                .Where(s => s.UserId == uid)
                .Select(s => new { s.Id, s.DepartmentId })
                .FirstOrDefaultAsync();
            if (staff is null)
                return Forbid();

            var appointment = await _db.Appointments.AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new { a.DoctorId })
                .FirstOrDefaultAsync();
            if (appointment is null)
                return NotFound(ApiResponse<AppointmentDto>.Fail("Appointment not found."));
            if (appointment.DoctorId != staff.Id)
                return Forbid();

            command = command with { DoctorId = staff.Id, DepartmentId = staff.DepartmentId };
        }

        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<AppointmentDto>.Ok(dto));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<object>>> ChangeStatus(int id, [FromQuery] AppointmentStatus status)
    {
        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staffId = await _db.StaffMembers.AsNoTracking()
                .Where(s => s.UserId == uid)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
            if (staffId != 0)
            {
                var appointment = await _db.Appointments.AsNoTracking()
                    .Where(a => a.Id == id)
                    .Select(a => a.DoctorId)
                    .FirstOrDefaultAsync();
                if (appointment is null)
                    return NotFound(ApiResponse<object>.Fail("Appointment not found."));
                if (appointment != staffId)
                    return Forbid();
            }
        }

        await _mediator.Send(new ChangeAppointmentStatusCommand(id, status));
        return Ok(ApiResponse<object>.Ok(new { }));
    }
}

