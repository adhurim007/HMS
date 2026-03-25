using HrmsH.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Localization;

public sealed record GetLanguagesQuery() : IRequest<IReadOnlyList<LanguageDto>>;

public sealed class GetLanguagesQueryHandler
    : IRequestHandler<GetLanguagesQuery, IReadOnlyList<LanguageDto>>
{
    private readonly IHrmsDbContext _db;

    public GetLanguagesQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<LanguageDto>> Handle(
        GetLanguagesQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.Languages
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new LanguageDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                IsDefault = x.IsDefault,
                IsActive = x.IsActive,
            })
            .ToListAsync(cancellationToken);
    }
}

public sealed record GetTranslationsQuery(string LanguageCode)
    : IRequest<IReadOnlyList<TranslationDto>>;

public sealed class GetTranslationsQueryHandler
    : IRequestHandler<GetTranslationsQuery, IReadOnlyList<TranslationDto>>
{
    private readonly IHrmsDbContext _db;

    public GetTranslationsQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<TranslationDto>> Handle(
        GetTranslationsQuery request,
        CancellationToken cancellationToken)
    {
        var code = request.LanguageCode.ToLowerInvariant();
        return await _db.Translations
            .AsNoTracking()
            .Where(x => x.LanguageCode == code)
            .OrderBy(x => x.Key)
            .Select(x => new TranslationDto
            {
                Id = x.Id,
                LanguageCode = x.LanguageCode,
                Key = x.Key,
                Value = x.Value,
            })
            .ToListAsync(cancellationToken);
    }
}

public sealed record GetTranslationsDictionaryQuery(string LanguageCode)
    : IRequest<IDictionary<string, string>>;

public sealed class GetTranslationsDictionaryQueryHandler
    : IRequestHandler<GetTranslationsDictionaryQuery, IDictionary<string, string>>
{
    private readonly IHrmsDbContext _db;

    public GetTranslationsDictionaryQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IDictionary<string, string>> Handle(
        GetTranslationsDictionaryQuery request,
        CancellationToken cancellationToken)
    {
        var code = request.LanguageCode.ToLowerInvariant();
        return await _db.Translations
            .AsNoTracking()
            .Where(x => x.LanguageCode == code)
            .ToDictionaryAsync(x => x.Key, x => x.Value, cancellationToken);
    }
}

