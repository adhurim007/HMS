using HrmsH.Domain.Common;

namespace HrmsH.Domain.Localization;

public class Translation : BaseEntity
{
    public string LanguageCode { get; set; } = default!;
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
}

