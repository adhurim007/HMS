using HrmsH.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HrmsH.Infrastructure.Persistence;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
public sealed class RolesController : ControllerBase
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RolesController(RoleManager<ApplicationRole> roleManager) => _roleManager = roleManager;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RoleDto>>>> GetList()
    {
        var roles = _roleManager.Roles.OrderBy(r => r.Name);
        var list = new List<RoleDto>();
        foreach (var r in roles)
        {
            list.Add(new RoleDto { Id = r.Id, Name = r.Name ?? "" });
        }
        return Ok(ApiResponse<IReadOnlyList<RoleDto>>.Ok(list));
    }
}

public sealed class RoleDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
}
