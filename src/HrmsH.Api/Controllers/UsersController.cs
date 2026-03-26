using HrmsH.Api.Models;
using HrmsH.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsH.Infrastructure.Persistence;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
public sealed class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public UsersController(UserManager<ApplicationUser> userManager, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<UserListDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        var query = _userManager.Users.AsQueryable();
        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hospitalId)
        {
            query = query.Where(u => u.HospitalId == hospitalId);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(u =>
                (u.Email != null && u.Email.ToLower().Contains(term)) ||
                (u.UserName != null && u.UserName.ToLower().Contains(term)));
        }

        var total = await query.CountAsync();
        var users = await query
            .OrderBy(u => u.Email)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = new List<UserListDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            items.Add(new UserListDto
            {
                Id = user.Id,
                Email = user.Email ?? user.UserName ?? user.Id.ToString(),
                Roles = roles.ToList(),
                LockoutEnd = user.LockoutEnd?.UtcDateTime
            });
        }

        return Ok(new PagedApiResponse<UserListDto>
        {
            Success = true,
            Items = items,
            TotalCount = total
        });
    }

    [HttpPost("{id:int}/reset-password")]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword(
        [FromRoute] int id,
        [FromBody] ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest(ApiResponse<object>.Fail("New password is required."));

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
            return NotFound(ApiResponse<object>.Fail("User not found."));

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<object>.Fail("Failed to reset password.", result.Errors));

        return Ok(ApiResponse<object>.Ok(new { }));
    }
}

public sealed class UserListDto
{
    public int Id { get; init; }
    public required string Email { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    public DateTime? LockoutEnd { get; init; }
}

public sealed class ResetPasswordRequest
{
    public required string NewPassword { get; init; }
}
