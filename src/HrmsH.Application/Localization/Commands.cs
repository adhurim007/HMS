using HrmsH.Application.Abstractions;
using HrmsH.Domain.Localization;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Localization;

public sealed record UpsertLanguageCommand(
    int? Id,
    string Code,
    string Name,
    bool IsDefault,
    bool IsActive) : IRequest<LanguageDto>;

public sealed class UpsertLanguageCommandHandler
    : IRequestHandler<UpsertLanguageCommand, LanguageDto>
{
    private readonly IHrmsDbContext _db;

    public UpsertLanguageCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<LanguageDto> Handle(
        UpsertLanguageCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.ToLowerInvariant();

        Language entity;
        if (request.Id.HasValue)
        {
            entity = await _db.Languages
                .FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Language not found.");
        }
        else
        {
            entity = new Language
            {
                Code = code,
                Name = request.Name,
                IsDefault = request.IsDefault,
                IsActive = request.IsActive,
            };
            _db.Languages.Add(entity);
        }

        entity.Name = request.Name;
        entity.IsActive = request.IsActive;
        entity.IsDefault = request.IsDefault;
        entity.Code = code;

        if (request.IsDefault)
        {
            foreach (var other in await _db.Languages
                         .Where(x => x.Id != entity.Id)
                         .ToListAsync(cancellationToken))
            {
                other.IsDefault = false;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new LanguageDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            IsDefault = entity.IsDefault,
            IsActive = entity.IsActive,
        };
    }
}

public sealed record UpsertTranslationsCommand(
    string LanguageCode,
    IReadOnlyList<TranslationDto> Items) : IRequest<IReadOnlyList<TranslationDto>>;

public sealed class UpsertTranslationsCommandHandler
    : IRequestHandler<UpsertTranslationsCommand, IReadOnlyList<TranslationDto>>
{
    private readonly IHrmsDbContext _db;

    public UpsertTranslationsCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<TranslationDto>> Handle(
        UpsertTranslationsCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.LanguageCode.ToLowerInvariant();

        var existing = await _db.Translations
            .Where(x => x.LanguageCode == code)
            .ToListAsync(cancellationToken);

        foreach (var item in request.Items)
        {
            var key = item.Key;
            var value = item.Value;
            var entity = existing.FirstOrDefault(x => x.Key == key);
            if (entity is null)
            {
                entity = new Translation
                {
                    LanguageCode = code,
                    Key = key,
                    Value = value,
                };
                _db.Translations.Add(entity);
            }
            else
            {
                entity.Value = value;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

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

