using HrmsH.Api.Models;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Organization.Dtos;
using HrmsH.Application.Organization.Hospitals.Commands;
using HrmsH.Application.Organization.Hospitals.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public sealed class HospitalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public HospitalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<HospitalDto>>> Get(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        [FromQuery] string? search = null)
    {
        var result = await _mediator.Send(new GetHospitalsQuery(new PaginationParams
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDesc = sortDesc,
            Search = search
        }));

        return Ok(new PagedApiResponse<HospitalDto>
        {
            Success = true,
            Items = result.Items,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<HospitalDto>>> GetById(int id)
    {
        var dto = await _mediator.Send(new GetHospitalByIdQuery(id));
        return Ok(ApiResponse<HospitalDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<HospitalDto>>> Create([FromBody] CreateHospitalCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<HospitalDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<HospitalDto>>> Update(int id, [FromBody] UpdateHospitalCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<HospitalDto>.Fail("Route id does not match body id."));

        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<HospitalDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _mediator.Send(new DeleteHospitalCommand(id));
        return Ok(ApiResponse<object>.Ok(new { }));
    }
}
