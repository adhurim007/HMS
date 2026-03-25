namespace HrmsH.Application.Localization;

public sealed class LanguageDto
{
    public int Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public bool IsDefault { get; init; }
    public bool IsActive { get; init; }
}

public sealed class TranslationDto
{
    public int Id { get; init; }
    public required string LanguageCode { get; init; }
    public required string Key { get; init; }
    public required string Value { get; init; }
}

