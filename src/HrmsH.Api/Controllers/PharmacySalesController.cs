using HrmsH.Api.Models;
using HrmsH.Application.Billing.Invoices.Dtos;
using HrmsH.Application.Pharmacy.Sales.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Finance,Pharmacist,Manager")]
public sealed class PharmacySalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PharmacySalesController(IMediator mediator) => _mediator = mediator;

    [HttpPost("sell")]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> Sell(
        [FromBody] CreatePharmacySaleCommand command)
    {
        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<InvoiceDto>.Ok(dto));
    }
}

