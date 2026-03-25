using HrmsH.Api.Models;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Notifications;
using HrmsH.Application.Notifications.Commands;
using HrmsH.Application.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public NotificationsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NotificationDto>>>> GetList()
    {
        if (_currentUser.UserId is not int userId)
            return Ok(ApiResponse<IReadOnlyList<NotificationDto>>.Ok(Array.Empty<NotificationDto>()));

        var roleNames = User.FindAll(ClaimTypes.Role).Select(c => c.Value).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var list = await _mediator.Send(new GetNotificationsQuery(userId, roleNames));
        return Ok(ApiResponse<IReadOnlyList<NotificationDto>>.Ok(list));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
        if (_currentUser.UserId is not int userId)
            return Ok(ApiResponse<int>.Ok(0));

        var roleNames = User.FindAll(ClaimTypes.Role).Select(c => c.Value).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var count = await _mediator.Send(new GetUnreadNotificationsCountQuery(userId, roleNames));
        return Ok(ApiResponse<int>.Ok(count));
    }

    [HttpPatch("mark-read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkRead([FromBody] MarkNotificationReadRequest request)
    {
        if (_currentUser.UserId is not int userId)
            return Unauthorized(ApiResponse<object>.Fail("Not authenticated."));
        if (string.IsNullOrWhiteSpace(request?.Type) || string.IsNullOrWhiteSpace(request?.Key))
            return BadRequest(ApiResponse<object>.Fail("Type and Key are required."));

        await _mediator.Send(new MarkNotificationReadCommand(userId, request.Type.Trim(), request.Key.Trim()));
        return Ok(ApiResponse<object>.Ok(new { }));
    }

    public sealed class MarkNotificationReadRequest
    {
        public string? Type { get; set; }
        public string? Key { get; set; }
    }
}
