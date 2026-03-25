using HrmsH.Api.Models;
using HrmsH.Application.Audit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
public sealed class AuditController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedAuditResult>>> Get(
        [FromQuery] string? entityType,
        [FromQuery] int? patientId,
        [FromQuery] string? userName,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _mediator.Send(
            new GetAuditLogsQuery(
                entityType,
                patientId,
                userName,
                fromUtc,
                toUtc,
                pageNumber,
                pageSize));

        return Ok(ApiResponse<PagedAuditResult>.Ok(result));
    }
}

