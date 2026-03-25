using HrmsH.Api.Models;
using HrmsH.Application.Localization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrmsH.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,HospitalAdmin,Manager")]
public sealed class LocalizationController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocalizationController(IMediator mediator) => _mediator = mediator;

    [HttpGet("languages")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<LanguageDto>>>> GetLanguages()
    {
        var list = await _mediator.Send(new GetLanguagesQuery());
        return Ok(ApiResponse<IReadOnlyList<LanguageDto>>.Ok(list));
    }

    [HttpPost("languages")]
    public async Task<ActionResult<ApiResponse<LanguageDto>>> UpsertLanguage(
        [FromBody] UpsertLanguageCommand command)
    {
        var dto = await _mediator.Send(command);
        return Ok(ApiResponse<LanguageDto>.Ok(dto));
    }

    [HttpGet("{code}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IDictionary<string, string>>>> GetTranslations(
        [FromRoute] string code)
    {
        var dict = await _mediator.Send(new GetTranslationsDictionaryQuery(code));
        return Ok(ApiResponse<IDictionary<string, string>>.Ok(dict));
    }

    [HttpGet("{code}/entries")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TranslationDto>>>> GetEntries(
        [FromRoute] string code)
    {
        var list = await _mediator.Send(new GetTranslationsQuery(code));
        return Ok(ApiResponse<IReadOnlyList<TranslationDto>>.Ok(list));
    }

    [HttpPost("{code}/entries")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TranslationDto>>>> UpsertEntries(
        [FromRoute] string code,
        [FromBody] IReadOnlyList<TranslationDto> items)
    {
        var list = await _mediator.Send(new UpsertTranslationsCommand(code, items));
        return Ok(ApiResponse<IReadOnlyList<TranslationDto>>.Ok(list));
    }
}

