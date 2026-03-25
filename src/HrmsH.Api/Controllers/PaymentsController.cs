using HrmsH.Api.Models;
using HrmsH.Application.Billing.Invoices.Dtos;
using HrmsH.Application.Billing.Payments.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Finance,Manager")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> AddPayment([FromBody] AddPaymentCommand command)
    {
        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<PaymentDto>.Ok(dto));
    }
}
