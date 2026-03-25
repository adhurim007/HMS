using HrmsH.Api.Models;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Organization.Dtos;
using HrmsH.Application.Organization.Facilities.Commands;
using HrmsH.Application.Organization.Facilities.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
public sealed class FacilitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FacilitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<FacilityDto>>> Get(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        [FromQuery] string? search = null)
    {
        var result = await _mediator.Send(new GetFacilitiesQuery(new PaginationParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDesc = sortDesc,
            Search = search
        }));

        return Ok(new PagedApiResponse<FacilityDto>
        {
            Success = true,
            Items = result.Items,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<FacilityDto>>> GetById(int id)
    {
        var dto = await _mediator.Send(new GetFacilityByIdQuery(id));
        return Ok(ApiResponse<FacilityDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FacilityDto>>> Create([FromBody] CreateFacilityCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<FacilityDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<FacilityDto>>> Update(int id, [FromBody] UpdateFacilityCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<FacilityDto>.Fail("Route id does not match body id."));

        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<FacilityDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _mediator.Send(new DeleteFacilityCommand(id));
        return Ok(ApiResponse<object>.Ok(new { }));
    }
}

