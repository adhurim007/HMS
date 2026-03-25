using HrmsH.Api.Models;
using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ProfileController(
        UserManager<ApplicationUser> userManager,
        IHrmsDbContext db,
        ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _db = db;
        _currentUser = currentUser;
    }

    public sealed class ProfileDto
    {
        public int Id { get; init; }
        public required string Email { get; init; }
        public string? FullName { get; init; }
        public string? Phone { get; init; }
    }

    public sealed class UpdateProfileRequest
    {
        public string? Email { get; init; }
        public string? FullName { get; init; }
        public string? Phone { get; init; }
    }

    public sealed class ChangePasswordRequest
    {
        public required string CurrentPassword { get; init; }
        public required string NewPassword { get; init; }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<ProfileDto>>> Get()
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized(ApiResponse<ProfileDto>.Fail("Not authenticated."));
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound(ApiResponse<ProfileDto>.Fail("User not found."));
        }

        var staff = await _db.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId);

        var dto = new ProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? user.UserName ?? string.Empty,
            FullName = staff?.FullName,
            Phone = staff?.Phone
        };

        return Ok(ApiResponse<ProfileDto>.Ok(dto));
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<ProfileDto>>> Update([FromBody] UpdateProfileRequest request)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized(ApiResponse<ProfileDto>.Fail("Not authenticated."));
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound(ApiResponse<ProfileDto>.Fail("User not found."));
        }

        if (!string.IsNullOrWhiteSpace(request.Email) &&
            !string.Equals(request.Email, user.Email, StringComparison.OrdinalIgnoreCase))
        {
            user.Email = request.Email;
            user.UserName = request.Email;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(ApiResponse<ProfileDto>.Fail("Failed to update email.", updateResult.Errors));
            }
        }

        var staff = await _db.StaffMembers
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staff is not null)
        {
            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                staff.FullName = request.FullName.Trim();
            }

            if (request.Phone is not null)
            {
                staff.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
            }

            await _db.SaveChangesAsync();
        }

        var dto = new ProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? user.UserName ?? string.Empty,
            FullName = staff?.FullName,
            Phone = staff?.Phone
        };

        return Ok(ApiResponse<ProfileDto>.Ok(dto));
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized(ApiResponse<object>.Fail("Not authenticated."));
        }

        if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
            string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return BadRequest(ApiResponse<object>.Fail("Both current and new password are required."));
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return NotFound(ApiResponse<object>.Fail("User not found."));
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse<object>.Fail("Failed to change password.", result.Errors));
        }

        return Ok(ApiResponse<object>.Ok(new { }));
    }
}

