using HrmsH.Domain.Common;

namespace HrmsH.Domain.Localization;

public class Language : BaseEntity
{
    public string Code { get; set; } = default!; // e.g. "en", "sq"
    public string Name { get; set; } = default!; // e.g. "English", "Shqip"
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
}

