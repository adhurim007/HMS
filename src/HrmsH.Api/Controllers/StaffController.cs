using HrmsH.Api.Models;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Staff.Commands;
using HrmsH.Application.Staff.Dtos;
using HrmsH.Application.Staff.Queries;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
public sealed class StaffController : ControllerBase
{
    private readonly IMediator _mediator;

    public StaffController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<StaffMemberDto>>> Get(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        [FromQuery] string? search = null,
        [FromQuery] StaffType? staffType = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetStaffMembersQuery(
            new PaginationParams
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDesc = sortDesc,
                Search = search
            },
            staffType,
            departmentId,
            isActive));

        return Ok(new PagedApiResponse<StaffMemberDto>
        {
            Success = true,
            Items = result.Items,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<StaffMemberDto>>> GetById(int id)
    {
        var dto = await _mediator.Send(new GetStaffMemberByIdQuery(id));
        return Ok(ApiResponse<StaffMemberDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<StaffMemberDto>>> Create([FromBody] CreateStaffMemberCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<StaffMemberDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<StaffMemberDto>>> Update(int id, [FromBody] UpdateStaffMemberCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<StaffMemberDto>.Fail("Route id does not match body id."));

        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<StaffMemberDto>.Ok(dto));
    }

    [HttpPatch("{id:int}/active")]
    public async Task<ActionResult<ApiResponse<object>>> SetActive(int id, [FromQuery] bool isActive)
    {
        await _mediator.Send(new ToggleStaffActiveCommand(id, isActive));
        return Ok(ApiResponse<object>.Ok(new { }));
    }
}

