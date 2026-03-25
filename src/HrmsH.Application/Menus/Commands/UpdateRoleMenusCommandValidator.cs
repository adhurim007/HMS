using FluentValidation;

namespace HrmsH.Application.Menus.Commands;

public sealed class UpdateRoleMenusCommandValidator : AbstractValidator<UpdateRoleMenusCommand>
{
    public UpdateRoleMenusCommandValidator()
    {
        RuleFor(x => x.RoleId).GreaterThan(0);
    }
}
