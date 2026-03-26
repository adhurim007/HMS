using HrmsH.Api.Models;
using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Staff.Commands;
using HrmsH.Application.Staff.Doctors.Commands;
using HrmsH.Application.Staff.Doctors.Queries;
using HrmsH.Application.Staff.Doctors.Dtos;
using HrmsH.Application.Staff.Dtos;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsH.Infrastructure.Persistence;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager,Doctor,Reception")]
public sealed class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICurrentUserService _currentUser;
    private readonly IHrmsDbContext _db;

    public DoctorsController(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ICurrentUserService currentUser,
        IHrmsDbContext db)
    {
        _mediator = mediator;
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUser = currentUser;
        _db = db;
    }

    /// <summary>Current logged-in doctor's profile (for appointment form: self + department).</summary>
    [HttpGet("me")]
    [Authorize(Roles = "Doctor")]
    public async Task<ActionResult<ApiResponse<DoctorMeDto>>> GetMe()
    {
        if (_currentUser.UserId is not int uid)
            return NotFound(ApiResponse<DoctorMeDto>.Fail("User not found."));

        var staff = await _db.StaffMembers
            .AsNoTracking()
            .Where(s => s.UserId == uid && s.StaffType == StaffType.Doctor)
            .Select(s => new { s.Id, s.FullName, s.DepartmentId })
            .FirstOrDefaultAsync();

        if (staff is null)
            return NotFound(ApiResponse<DoctorMeDto>.Fail("Current user is not a doctor."));

        string? departmentName = null;
        if (staff.DepartmentId is int depId)
        {
            departmentName = await _db.Departments
                .AsNoTracking()
                .Where(d => d.Id == depId)
                .Select(d => d.Name)
                .FirstOrDefaultAsync();
        }

        return Ok(ApiResponse<DoctorMeDto>.Ok(new DoctorMeDto
        {
            StaffMemberId = staff.Id,
            FullName = staff.FullName,
            DepartmentId = staff.DepartmentId,
            DepartmentName = departmentName
        }));
    }

    /// <summary>Create a new doctor and an Identity account (Doctor role) so they can sign in.</summary>
    [HttpPost("with-account")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> CreateDoctorWithAccount(
        [FromBody] CreateDoctorWithAccountRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(ApiResponse<DoctorDto>.Fail("Email and password are required."));

        var existingUser = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (existingUser is not null)
            return BadRequest(ApiResponse<DoctorDto>.Fail("A user with this email already exists."));

        var user = new ApplicationUser
        {
            UserName = request.Email.Trim(),
            Email = request.Email.Trim()
        };
        if (_currentUser.IsSuperAdmin)
        {
            if (request.FacilityId is int facilityIdForSuper)
            {
                user.HospitalId = await _db.Facilities
                    .AsNoTracking()
                    .Where(f => f.Id == facilityIdForSuper)
                    .Select(f => (int?)f.HospitalId)
                    .FirstOrDefaultAsync();
            }
        }
        else
        {
            user.HospitalId = await _db.Facilities
                .AsNoTracking()
                .Where(f => f.Id == request.FacilityId && f.HospitalId == _currentUser.HospitalId)
                .Select(f => (int?)f.HospitalId)
                .FirstOrDefaultAsync();
            if (user.HospitalId is null)
                return BadRequest(ApiResponse<DoctorDto>.Fail("Invalid facility for your hospital scope."));
        }
        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return BadRequest(ApiResponse<DoctorDto>.Fail("Failed to create account.", createResult.Errors));

        const string doctorRole = "Doctor";
        if (await _roleManager.RoleExistsAsync(doctorRole))
        {
            var roleResult = await _userManager.AddToRoleAsync(user, doctorRole);
            if (!roleResult.Succeeded)
                return BadRequest(ApiResponse<DoctorDto>.Fail("Failed to assign Doctor role.", roleResult.Errors));
        }

        var staffCommand = new CreateStaffMemberCommand(
            request.FullName.Trim(),
            StaffType.Doctor,
            string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            request.Email.Trim(),
            request.DepartmentId,
            user.Id,
            request.FacilityId is int facilityId ? new[] { facilityId } : null);

        var staffDto = await _mediator.Send(staffCommand);
        var profileCommand = new UpsertDoctorProfileCommand(
            staffDto.Id,
            string.IsNullOrWhiteSpace(request.Specialty) ? null : request.Specialty.Trim(),
            string.IsNullOrWhiteSpace(request.LicenseNumber) ? null : request.LicenseNumber.Trim());

        var doctorDto = await _mediator.Send(profileCommand);
        return CreatedAtAction(nameof(GetById), new { staffMemberId = doctorDto.StaffMemberId },
            ApiResponse<DoctorDto>.Ok(doctorDto));
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<DoctorDto>>> Get(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        [FromQuery] string? search = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetDoctorsQuery(
            new PaginationParams
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDesc = sortDesc,
                Search = search
            },
            departmentId,
            isActive));

        return Ok(new PagedApiResponse<DoctorDto>
        {
            Success = true,
            Items = result.Items,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{staffMemberId:int}")]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> GetById(int staffMemberId)
    {
        var dto = await _mediator.Send(new GetDoctorByIdQuery(staffMemberId));
        return Ok(ApiResponse<DoctorDto>.Ok(dto));
    }

    [HttpPost("{staffMemberId:int}/profile")]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> UpsertProfile(
        int staffMemberId,
        [FromBody] UpsertDoctorProfileCommand body)
    {
        if (staffMemberId != body.StaffMemberId)
            return BadRequest(ApiResponse<DoctorDto>.Fail("Route staffMemberId does not match body."));

        var dto = await _mediator.Send(body);
        return Ok(ApiResponse<DoctorDto>.Ok(dto));
    }

    /// <summary>Get doctor's visit settings (min duration minutes).</summary>
    [HttpGet("{staffMemberId:int}/visit-settings")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager,Doctor,Reception")]
    public async Task<ActionResult<ApiResponse<DoctorVisitSettingsDto?>>> GetVisitSettings(
        int staffMemberId,
        CancellationToken cancellationToken)
    {
        // Doctors can only access their own schedule settings.
        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staffId = await _db.StaffMembers
                .AsNoTracking()
                .Where(s => s.UserId == uid && s.StaffType == StaffType.Doctor)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (staffId != staffMemberId)
                return Forbid();
        }

        var dto = await _mediator.Send(
            new GetDoctorVisitSettingsByDoctorIdQuery(staffMemberId),
            cancellationToken);

        return Ok(ApiResponse<DoctorVisitSettingsDto?>.Ok(dto));
    }

    /// <summary>Upsert doctor's visit settings (min duration minutes).</summary>
    [HttpPost("{staffMemberId:int}/visit-settings")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager,Doctor,Reception")]
    public async Task<ActionResult<ApiResponse<DoctorVisitSettingsDto>>> UpsertVisitSettings(
        int staffMemberId,
        [FromBody] UpsertDoctorVisitSettingsCommand body,
        CancellationToken cancellationToken)
    {
        if (staffMemberId != body.StaffMemberId)
            return BadRequest(ApiResponse<DoctorVisitSettingsDto>.Fail("Route staffMemberId does not match body.StaffMemberId."));

        // Doctors can only access their own schedule settings.
        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staffId = await _db.StaffMembers
                .AsNoTracking()
                .Where(s => s.UserId == uid && s.StaffType == StaffType.Doctor)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (staffId != staffMemberId)
                return Forbid();
        }

        var dto = await _mediator.Send(body with { StaffMemberId = staffMemberId }, cancellationToken);
        return Ok(ApiResponse<DoctorVisitSettingsDto>.Ok(dto));
    }

    /// <summary>Get doctor's weekly schedule (working days + hours).</summary>
    [HttpGet("{staffMemberId:int}/weekly-schedule")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager,Doctor,Reception")]
    public async Task<ActionResult<ApiResponse<DoctorWeeklyScheduleDto>>> GetWeeklySchedule(
        int staffMemberId,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staffId = await _db.StaffMembers
                .AsNoTracking()
                .Where(s => s.UserId == uid && s.StaffType == StaffType.Doctor)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (staffId != staffMemberId)
                return Forbid();
        }

        var dto = await _mediator.Send(new GetDoctorWeeklyScheduleByDoctorIdQuery(staffMemberId), cancellationToken);
        return Ok(ApiResponse<DoctorWeeklyScheduleDto>.Ok(dto));
    }

    /// <summary>Upsert doctor's weekly schedule (working days + hours).</summary>
    [HttpPost("{staffMemberId:int}/weekly-schedule")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager,Doctor,Reception")]
    public async Task<ActionResult<ApiResponse<bool>>> UpsertWeeklySchedule(
        int staffMemberId,
        [FromBody] UpsertDoctorWeeklyScheduleCommand body,
        CancellationToken cancellationToken)
    {
        if (staffMemberId != body.StaffMemberId)
            return BadRequest(ApiResponse<bool>.Fail("Route staffMemberId does not match body.StaffMemberId."));

        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staffId = await _db.StaffMembers
                .AsNoTracking()
                .Where(s => s.UserId == uid && s.StaffType == StaffType.Doctor)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (staffId != staffMemberId)
                return Forbid();
        }

        var ok = await _mediator.Send(body with { StaffMemberId = staffMemberId }, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(ok));
    }

    /// <summary>Available calendar slots for a doctor in a date range.</summary>
    [HttpGet("{staffMemberId:int}/available-slots")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager,Doctor,Reception")]
    public async Task<ActionResult<ApiResponse<GetDoctorCalendarSlotsDto>>> GetAvailableSlots(
        int staffMemberId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole("Doctor") && !User.IsInRole("SuperAdmin") && _currentUser.UserId is int uid)
        {
            var staffId = await _db.StaffMembers
                .AsNoTracking()
                .Where(s => s.UserId == uid && s.StaffType == StaffType.Doctor)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (staffId != staffMemberId)
                return Forbid();
        }

        var dto = await _mediator.Send(
            new GetDoctorCalendarSlotsByDoctorIdQuery(staffMemberId, from.Date, to.Date),
            cancellationToken);

        return Ok(ApiResponse<GetDoctorCalendarSlotsDto>.Ok(dto));
    }
}

public sealed class DoctorMeDto
{
    public int StaffMemberId { get; init; }
    public required string FullName { get; init; }
    public int? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
}

public sealed class CreateDoctorWithAccountRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FullName { get; init; }
    public string? Phone { get; init; }
    public int? FacilityId { get; init; }
    public int? DepartmentId { get; init; }
    public string? Specialty { get; init; }
    public string? LicenseNumber { get; init; }
}

