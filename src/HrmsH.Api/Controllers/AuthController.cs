using HrmsH.Api.Auth;
using HrmsH.Api.Models;
using HrmsH.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtTokenService _tokenService;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AuthController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        JwtTokenService tokenService,
        RoleManager<ApplicationRole> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _tokenService = tokenService;
        _roleManager = roleManager;
    }

    public sealed record LoginRequest(string Email, string Password);
    public sealed record LoginResponse(string Token);
    public sealed record RegisterRequest(string Email, string Password);
    public sealed record CreateUserRequest(string Email, string Password, string Role);
    public sealed record CreateUserResponse(int Id, string Email);

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid credentials."));

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized(ApiResponse<LoginResponse>.Fail("Invalid credentials."));

        var token = await _tokenService.CreateTokenAsync(user);
        return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse(token)));
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register([FromBody] RegisterRequest request)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return BadRequest(ApiResponse<LoginResponse>.Fail("Email is already registered."));

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<LoginResponse>.Fail("Registration failed.", result.Errors));

        var token = await _tokenService.CreateTokenAsync(user);
        return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse(token)));
    }

    [HttpPost("create-user")]
    [Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
    public async Task<ActionResult<ApiResponse<CreateUserResponse>>> CreateUser([FromBody] CreateUserRequest request)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
        {
            return BadRequest(ApiResponse<CreateUserResponse>.Fail("Email is already registered."));
        }

        var roleExists = await _roleManager.RoleExistsAsync(request.Role);
        if (!roleExists)
        {
            return BadRequest(ApiResponse<CreateUserResponse>.Fail("Role does not exist."));
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(ApiResponse<CreateUserResponse>.Fail("Failed to create user.", createResult.Errors));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded)
        {
            return BadRequest(ApiResponse<CreateUserResponse>.Fail("Failed to assign role.", roleResult.Errors));
        }

        var dto = new CreateUserResponse(user.Id, user.Email ?? request.Email);
        return Ok(ApiResponse<CreateUserResponse>.Ok(dto));
    }
}

