using HrmsH.Domain.Common;

namespace HrmsH.Domain.Menus;

public class RoleMenu : BaseEntity
{
    public int RoleId { get; set; }
    public int MenuId { get; set; }

    public Menu Menu { get; set; } = default!;
}
