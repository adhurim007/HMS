using HrmsH.Api.Models;
using HrmsH.Application.Reports.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Reception,Finance,Pharmacist,Manager,Doctor")]
public sealed class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Get daily payment totals within a date range.</summary>
    [HttpGet("daily-payments")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DailyPaymentRowDto>>>> DailyPayments(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddDays(-30);
        var toDate = to ?? DateTime.UtcNow.Date;
        var result = await _mediator.Send(new DailyPaymentsReportQuery(fromDate, toDate));
        return Ok(ApiResponse<IReadOnlyList<DailyPaymentRowDto>>.Ok(result));
    }

    /// <summary>Get visit counts per doctor within a date range.</summary>
    [HttpGet("visits-per-doctor")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<VisitsPerDoctorRowDto>>>> VisitsPerDoctor(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var fromDate = from ?? DateTime.UtcNow.Date.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow.Date;
        var result = await _mediator.Send(new VisitsPerDoctorReportQuery(fromDate, toDate));
        return Ok(ApiResponse<IReadOnlyList<VisitsPerDoctorRowDto>>.Ok(result));
    }

    /// <summary>Get stock batches expiring within the given number of days.</summary>
    [HttpGet("stock-expiry-alerts")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<StockExpiryAlertRowDto>>>> StockExpiryAlerts(
        [FromQuery] int daysThreshold = 90)
    {
        var result = await _mediator.Send(new StockExpiryAlertsReportQuery(daysThreshold));
        return Ok(ApiResponse<IReadOnlyList<StockExpiryAlertRowDto>>.Ok(result));
    }
}
