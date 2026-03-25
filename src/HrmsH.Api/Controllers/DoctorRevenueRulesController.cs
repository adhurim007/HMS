using HrmsH.Api.Models;
using HrmsH.Application.Billing.DoctorRevenueRules.Commands;
using HrmsH.Application.Billing.DoctorRevenueRules.Dtos;
using HrmsH.Application.Billing.DoctorRevenueRules.Queries;
using HrmsH.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{SystemRoles.SuperAdmin},{SystemRoles.HospitalAdmin},{SystemRoles.Manager},{SystemRoles.Finance}")]
public sealed class DoctorRevenueRulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DoctorRevenueRulesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<DoctorRevenueRuleDto>>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new GetDoctorRevenueRulesQuery(), cancellationToken);
        return Ok(ApiResponse<List<DoctorRevenueRuleDto>>.Ok(items.ToList()));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DoctorRevenueRuleDto>>> Upsert(
        [FromBody] UpsertDoctorRevenueRuleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(ApiResponse<DoctorRevenueRuleDto>.Ok(result));
    }
}

