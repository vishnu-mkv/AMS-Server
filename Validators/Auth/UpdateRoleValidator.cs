using AMS.Interfaces;
using AMS.Requests;
using FluentValidation;

namespace AMS.Validators;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleValidator(IUserManager userManager, IRoleProvider roleManager, IAuthManager authManager)
    {

        RuleFor(x => x.Name)
            .MinimumLength(1).WithMessage("Role name must not be empty.")
            .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.");

        RuleFor(x => x.Permissions).SetValidator(new PermissionsValidator<UpdateRoleRequest>(roleManager));

        RuleFor(x => x.Users).SetValidator(new UsersValidator<UpdateRoleRequest>(
            authManager, userManager
        ));

        RuleFor(x => x.Color)
            .MustBeHexColor();

        RuleFor(x => x.Description)
            .MinimumLength(1).WithMessage("Description cannot be empty.").MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
    }
}