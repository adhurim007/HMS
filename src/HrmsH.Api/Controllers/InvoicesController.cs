using HrmsH.Api.Models;
using HrmsH.Application.Billing.Invoices.Commands;
using HrmsH.Application.Billing.Invoices.Dtos;
using HrmsH.Application.Billing.Invoices.Queries;
using HrmsH.Domain.Billing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Finance,Manager")]
public sealed class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<InvoiceListDto>>> GetList(
        [FromQuery] int? facilityId = null,
        [FromQuery] int? patientId = null,
        [FromQuery] InvoiceStatus? status = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = true)
    {
        var result = await _mediator.Send(new GetInvoicesQuery(facilityId, patientId, status, from, to, page, pageSize, sortBy, sortDescending));
        return Ok(new PagedApiResponse<InvoiceListDto>
        {
            Success = true,
            Items = result.Items,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("UnbilledVisitServices")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UnbilledVisitServiceDto>>>> GetUnbilledVisitServices(
        [FromQuery] int patientId,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int? doctorId = null)
    {
        var list = await _mediator.Send(new GetUnbilledVisitServicesQuery(patientId, from, to, doctorId));
        return Ok(ApiResponse<IReadOnlyList<UnbilledVisitServiceDto>>.Ok(list));
    }

    [HttpGet("UnbilledLaboratoryItems")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UnbilledLaboratoryItemDto>>>> GetUnbilledLaboratoryItems(
        [FromQuery] int patientId,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int? doctorId = null)
    {
        var list = await _mediator.Send(new GetUnbilledLaboratoryItemsQuery(patientId, from, to, doctorId));
        return Ok(ApiResponse<IReadOnlyList<UnbilledLaboratoryItemDto>>.Ok(list));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> GetById([FromRoute] int id)
    {
        var dto = await _mediator.Send(new GetInvoiceByIdQuery(id));
        if (dto is null)
            return NotFound(ApiResponse<InvoiceDto>.Fail("Invoice not found."));
        return Ok(ApiResponse<InvoiceDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> Create([FromBody] CreateInvoiceCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<InvoiceDto>.Ok(dto));
    }
}
