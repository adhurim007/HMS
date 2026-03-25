using HrmsH.Domain.Common;

namespace HrmsH.Domain.Diagnostics;

public class DiagnosticTest : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DiagnosticType Type { get; set; } = DiagnosticType.Lab;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}
