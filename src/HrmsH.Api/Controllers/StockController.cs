using HrmsH.Api.Models;
using HrmsH.Application.Pharmacy.Stock.Commands;
using HrmsH.Application.Pharmacy.Stock.Dtos;
using HrmsH.Application.Pharmacy.Stock.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Finance,Pharmacist,Manager")]
public sealed class StockController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockController(IMediator mediator) => _mediator = mediator;

    [HttpGet("batches/product/{productId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StockBatchDto>>>> GetBatchesByProduct([FromRoute] int productId)
    {
        var list = await _mediator.Send(new GetStockBatchesByProductQuery(productId));
        return Ok(ApiResponse<IReadOnlyList<StockBatchDto>>.Ok(list));
    }

    [HttpPost("batches")]
    public async Task<ActionResult<ApiResponse<StockBatchDto>>> CreateBatch([FromBody] CreateStockBatchCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBatchesByProduct), new { productId = dto.ProductId }, ApiResponse<StockBatchDto>.Ok(dto));
    }

    [HttpPost("movements")]
    public async Task<ActionResult<ApiResponse<StockMovementDto>>> RecordMovement([FromBody] RecordStockMovementCommand command)
    {
        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<StockMovementDto>.Ok(dto));
    }
}
