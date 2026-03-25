using HrmsH.Api.Models;
using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Dtos;
using HrmsH.Application.Billing.Services.Commands;
using HrmsH.Application.Billing.Services.Queries;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Domain.Billing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Finance,Manager,Doctor")]
public sealed class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ServicesController(IMediator mediator, IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _db = db;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<ServiceItemListDto>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetServiceItemsQuery(search, isActive, page, pageSize, sortBy, sortDescending));
        return Ok(new PagedApiResponse<ServiceItemListDto>
        {
            Success = true,
            Items = result.Items,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ServiceItemDto>>> GetById([FromRoute] int id)
    {
        var dto = await _mediator.Send(new GetServiceItemByIdQuery(id));
        if (dto is null)
            return NotFound(ApiResponse<ServiceItemDto>.Fail("Service item not found."));
        return Ok(ApiResponse<ServiceItemDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ServiceItemDto>>> Create([FromBody] CreateServiceItemCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<ServiceItemDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ServiceItemDto>>> Update([FromRoute] int id, [FromBody] UpdateServiceItemCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<ServiceItemDto>.Fail("Route id does not match body id."));
        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<ServiceItemDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromRoute] int id)
    {
        await _mediator.Send(new DeleteServiceItemCommand(id));
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    public sealed class UpdateServiceAssignmentsRequest
    {
        public IReadOnlyList<int>? ServiceItemIds { get; set; }
    }

    [HttpGet("department/{departmentId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<int>>>> GetForDepartment(int departmentId)
    {
        var ids = await _db.DepartmentServices
            .AsNoTracking()
            .Where(x => x.DepartmentId == departmentId)
            .Select(x => x.ServiceItemId)
            .ToListAsync();

        return Ok(ApiResponse<IReadOnlyList<int>>.Ok(ids));
    }

    [HttpPut("department/{departmentId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateForDepartment(
        int departmentId,
        [FromBody] UpdateServiceAssignmentsRequest request)
    {
        var newIds = (request.ServiceItemIds ?? Array.Empty<int>()).ToHashSet();

        var existing = await _db.DepartmentServices
            .Where(x => x.DepartmentId == departmentId)
            .ToListAsync();

        _db.DepartmentServices.RemoveRange(existing);
        foreach (var id in newIds)
        {
            _db.DepartmentServices.Add(new DepartmentService
            {
                DepartmentId = departmentId,
                ServiceItemId = id,
            });
        }

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpGet("doctor/{staffMemberId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<int>>>> GetForDoctor(int staffMemberId)
    {
        var ids = await _db.DoctorServices
            .AsNoTracking()
            .Where(x => x.StaffMemberId == staffMemberId)
            .Select(x => x.ServiceItemId)
            .ToListAsync();

        return Ok(ApiResponse<IReadOnlyList<int>>.Ok(ids));
    }

    [HttpPut("doctor/{staffMemberId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateForDoctor(
        int staffMemberId,
        [FromBody] UpdateServiceAssignmentsRequest request)
    {
        var newIds = (request.ServiceItemIds ?? Array.Empty<int>()).ToHashSet();

        var existing = await _db.DoctorServices
            .Where(x => x.StaffMemberId == staffMemberId)
            .ToListAsync();

        _db.DoctorServices.RemoveRange(existing);
        foreach (var id in newIds)
        {
            _db.DoctorServices.Add(new DoctorService
            {
                StaffMemberId = staffMemberId,
                ServiceItemId = id,
            });
        }

        await _db.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpGet("for-me")]
    [Authorize(Roles = "Doctor")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ServiceItemListDto>>>> GetForMe()
    {
        if (_currentUser.UserId is not int uid)
            return Ok(ApiResponse<IReadOnlyList<ServiceItemListDto>>.Ok(Array.Empty<ServiceItemListDto>()));

        var staff = await _db.StaffMembers.AsNoTracking()
            .Where(s => s.UserId == uid)
            .Select(s => new { s.Id, s.DepartmentId })
            .FirstOrDefaultAsync();

        if (staff is null)
            return Ok(ApiResponse<IReadOnlyList<ServiceItemListDto>>.Ok(Array.Empty<ServiceItemListDto>()));

        var doctorIds = await _db.DoctorServices
            .AsNoTracking()
            .Where(x => x.StaffMemberId == staff.Id)
            .Select(x => x.ServiceItemId)
            .ToListAsync();

        var deptIds = staff.DepartmentId is int depId
            ? await _db.DepartmentServices.AsNoTracking()
                .Where(x => x.DepartmentId == depId)
                .Select(x => x.ServiceItemId)
                .ToListAsync()
            : new List<int>();

        var allowedIds = doctorIds.Concat(deptIds).Distinct().ToList();

        var items = await _db.ServiceItems
            .AsNoTracking()
            .Where(x => allowedIds.Contains(x.Id) && x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new ServiceItemListDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Price = x.Price,
                IsActive = x.IsActive,
            })
            .ToListAsync();

        return Ok(ApiResponse<IReadOnlyList<ServiceItemListDto>>.Ok(items));
    }
}
