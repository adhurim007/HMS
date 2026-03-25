using HrmsH.Api.Models;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Organization.Departments.Commands;
using HrmsH.Application.Organization.Departments.Queries;
using HrmsH.Application.Organization.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager,Reception")]
public sealed class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepartmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<DepartmentDto>>> Get(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        [FromQuery] string? search = null,
        [FromQuery] int? facilityId = null)
    {
        var result = await _mediator.Send(new GetDepartmentsQuery(
            new PaginationParams
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDesc = sortDesc,
                Search = search
            },
            facilityId));

        return Ok(new PagedApiResponse<DepartmentDto>
        {
            Success = true,
            Items = result.Items,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetById(int id)
    {
        var dto = await _mediator.Send(new GetDepartmentByIdQuery(id));
        return Ok(ApiResponse<DepartmentDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Create([FromBody] CreateDepartmentCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<DepartmentDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Update(int id, [FromBody] UpdateDepartmentCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<DepartmentDto>.Fail("Route id does not match body id."));

        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<DepartmentDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _mediator.Send(new DeleteDepartmentCommand(id));
        return Ok(ApiResponse<object>.Ok(new { }));
    }
}

