using AMS.Interfaces;
using AMS.Requests;
using FluentValidation;

namespace AMS.Validators;

public class AddRoleValidator : AbstractValidator<AddRoleRequest>
{
    public AddRoleValidator(IUserManager userManager, IRoleProvider roleManager, IAuthManager authManager)
    {

        // check if role with id exists
        RuleFor(x => x.Id).Custom((id, context) =>
        {
            if (id == null) return;

            bool role = roleManager.CheckRolesExists(new[] { id });

            if (role)
            {
                context.AddFailure("Role with id " + id + " does not exist.");
            }
        });

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name must not be empty.")
            .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.");

        RuleFor(x => x.Permissions).SetValidator(new PermissionsValidator<AddRoleRequest>(roleManager));

        RuleFor(x => x.Users).SetValidator(new UsersValidator<AddRoleRequest>(
            authManager, userManager
        ));

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description cannot be empty.").MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
    }
}