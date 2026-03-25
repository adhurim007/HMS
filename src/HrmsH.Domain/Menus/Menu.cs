using HrmsH.Domain.Common;

namespace HrmsH.Domain.Menus;

public class Menu : BaseEntity
{
    public string Name { get; set; } = default!;
    public string MenuKey { get; set; } = default!;
    public string? Url { get; set; }
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; } = true;

    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } = new List<Menu>();
}
