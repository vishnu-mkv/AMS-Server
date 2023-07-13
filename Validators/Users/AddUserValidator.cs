using AMS.Interfaces;
using AMS.Requests;
using FluentValidation;

namespace AMS.Validators
{
    public class UserValidator : AbstractValidator<AddUserRequest>
    {
        public UserValidator(IRoleProvider rolePermissionProvider, IUserManager userManager, IScheduleManager scheduleManager, IAuthManager authManager, IGroupManager groupManager)
        {
            int max = 20;
            RuleFor(x => x.FirstName).NotEmpty().Length(2, max).WithMessage("First name is required.");
            RuleFor(x => x.LastName).NotEmpty().Length(1, max).WithMessage("Last name is required.");
            RuleFor(x => x.DOB).LessThan(DateTime.Today).WithMessage("Date of birth cannot be in the future.");
            RuleFor(x => x.SignInAllowed).NotNull().WithMessage("Sign in allowed is required.");

            RuleFor(x => x.UserName)
                .NotEmpty().Length(2, max).WithMessage("Username is required.")
                .When(x => x.SignInAllowed);

            RuleFor(x => x.Password)
                .NotEmpty().Length(2, max).WithMessage("Password is required.")
                .When(x => x.SignInAllowed);

            RuleFor(x => x.Id)
                .Must((user, id) => userManager.IsUniqueID(id));

            RuleFor(x => x.RoleIds)
                .SetValidator(new RolesValidator<AddUserRequest>(rolePermissionProvider, authManager));

            RuleFor(x => x.ScheduleId)
                .SetValidator(new ScheduleIdValidator<AddUserRequest>(scheduleManager, authManager));

            RuleFor(x => x.GroupIds)
                .SetValidator(new GroupsValidator<AddUserRequest>(authManager, groupManager));
        }
    }
}
