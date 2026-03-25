using HrmsH.Api.Models;
using HrmsH.Application.Pharmacy.Purchases.Commands;
using HrmsH.Application.Pharmacy.Purchases.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Finance,Pharmacist,Manager")]
public sealed class PharmacyPurchasesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PharmacyPurchasesController(IMediator mediator) => _mediator = mediator;

    // Create an incoming purchase invoice and add purchased stock batches.
    [HttpPost]
    public async Task<ActionResult<ApiResponse<PharmacyPurchaseInvoiceDto>>> Create(
        [FromBody] CreatePurchaseInvoiceCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(Create), new { id = dto.Id }, ApiResponse<PharmacyPurchaseInvoiceDto>.Ok(dto));
    }
}

