using HrmsH.Api.Models;
using HrmsH.Application.Pharmacy.Products.Commands;
using HrmsH.Application.Pharmacy.Products.Dtos;
using HrmsH.Application.Pharmacy.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Finance,Pharmacist,Manager,Doctor")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<ProductListDto>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _mediator.Send(new GetProductsQuery(search, isActive, page, pageSize, sortBy, sortDescending));
        return Ok(new PagedApiResponse<ProductListDto>
        {
            Success = true,
            Items = result.Items,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById([FromRoute] int id)
    {
        var dto = await _mediator.Send(new GetProductByIdQuery(id));
        if (dto is null)
            return NotFound(ApiResponse<ProductDto>.Fail("Product not found."));
        return Ok(ApiResponse<ProductDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<ProductDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update([FromRoute] int id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<ProductDto>.Fail("Route id does not match body id."));
        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<ProductDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromRoute] int id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return Ok(ApiResponse<object>.Ok(new { }));
    }
}
