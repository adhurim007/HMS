using HrmsH.Api.Models;
using HrmsH.Application.Billing.Installments.Commands;
using HrmsH.Application.Billing.Installments.Dtos;
using HrmsH.Application.Billing.Installments.Queries;
using HrmsH.Application.Billing.Invoices.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Finance,Manager")]
public sealed class InstallmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InstallmentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("plans")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<InstallmentPlanDto>>>> GetPlans(
        [FromQuery] int? facilityId = null,
        [FromQuery] int? patientId = null,
        [FromQuery] int? invoiceId = null)
    {
        var list = await _mediator.Send(new GetInstallmentPlansQuery(facilityId, patientId, invoiceId));
        return Ok(ApiResponse<IReadOnlyList<InstallmentPlanDto>>.Ok(list));
    }

    [HttpPost("plans")]
    public async Task<ActionResult<ApiResponse<InstallmentPlanDto>>> CreatePlan([FromBody] CreateInstallmentPlanCommand command)
    {
        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<InstallmentPlanDto>.Ok(dto));
    }

    [HttpPost("payments")]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> AddInstallmentPayment([FromBody] AddInstallmentPaymentCommand command)
    {
        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<PaymentDto>.Ok(dto));
    }

    [HttpGet("patient-history/{patientId:int}")]
    public async Task<ActionResult<ApiResponse<PatientPaymentHistoryDto>>> GetPatientHistory([FromRoute] int patientId, [FromQuery] int? facilityId = null)
    {
        var dto = await _mediator.Send(new GetPatientPaymentHistoryQuery(patientId, facilityId));
        return Ok(ApiResponse<PatientPaymentHistoryDto>.Ok(dto));
    }
}
