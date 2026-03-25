using HrmsH.Api.Models;
using HrmsH.Application.Menus.Commands;
using HrmsH.Application.Menus.Dtos;
using HrmsH.Application.Menus.Queries;
using HrmsH.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class MenusController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public MenusController(IMediator mediator, RoleManager<ApplicationRole> roleManager)
    {
        _mediator = mediator;
        _roleManager = roleManager;
    }

    /// <summary>Returns menus assigned to the current user's roles. Callable by any authenticated user.</summary>
    [HttpGet("my-menus")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MenuDto>>>> GetMyMenus()
    {
        var roleNames = User.FindAll(ClaimTypes.Role).Select(c => c.Value).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        if (roleNames.Count == 0)
            return Ok(ApiResponse<IReadOnlyList<MenuDto>>.Ok(Array.Empty<MenuDto>()));

        var roleIds = await _roleManager.Roles
            .Where(r => roleNames.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync();
        if (roleIds.Count == 0)
            return Ok(ApiResponse<IReadOnlyList<MenuDto>>.Ok(Array.Empty<MenuDto>()));

        var list = await _mediator.Send(new GetMenusForCurrentUserQuery(roleIds));
        return Ok(ApiResponse<IReadOnlyList<MenuDto>>.Ok(list));
    }

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MenuDto>>>> GetList([FromQuery] bool? isActive = null)
    {
        var list = await _mediator.Send(new GetMenusQuery(isActive));
        return Ok(ApiResponse<IReadOnlyList<MenuDto>>.Ok(list));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> GetById([FromRoute] int id)
    {
        var dto = await _mediator.Send(new GetMenuByIdQuery(id));
        if (dto is null)
            return NotFound(ApiResponse<MenuDto>.Fail("Menu not found."));
        return Ok(ApiResponse<MenuDto>.Ok(dto));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Create([FromBody] CreateMenuCommand command)
    {
        var dto = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<MenuDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<MenuDto>>> Update([FromRoute] int id, [FromBody] UpdateMenuCommand command)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<MenuDto>.Fail("Route id does not match body id."));
        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<MenuDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromRoute] int id)
    {
        await _mediator.Send(new DeleteMenuCommand(id));
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    [HttpGet("for-role/{roleId:int}")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MenuForRoleDto>>>> GetMenusForRole([FromRoute] int roleId)
    {
        var list = await _mediator.Send(new GetMenusForRoleQuery(roleId));
        return Ok(ApiResponse<IReadOnlyList<MenuForRoleDto>>.Ok(list));
    }

    [HttpPut("for-role/{roleId:int}")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMenusForRole(
        [FromRoute] int roleId,
        [FromBody] UpdateRoleMenusRequest request)
    {
        await _mediator.Send(new UpdateRoleMenusCommand(roleId, request.MenuIds ?? Array.Empty<int>()));
        return Ok(ApiResponse<object>.Ok(new { }));
    }
}

public sealed class UpdateRoleMenusRequest
{
    public IReadOnlyList<int>? MenuIds { get; set; }
}
