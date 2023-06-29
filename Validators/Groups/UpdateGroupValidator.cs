using AMS.Interfaces;
using AMS.Requests;
using FluentValidation;

namespace AMS.Validators;

public class UpdateGroupValidaor : AbstractValidator<UpdateGroupRequest>
{
    public UpdateGroupValidaor(IUserManager userManager, IAuthManager authManager, IGroupManager groupManager)
    {

        RuleFor(x => x.Name).MinimumLength(3).WithMessage("Name must be at least 3 characters");

        RuleFor(x => x.Users_to_add).SetValidator(
            new UsersValidator<UpdateGroupRequest>(authManager, userManager)
        );

        // when group type is GroupOfGroups, groups must be provided else groups should be null
        RuleFor(x => x.Groups_to_add).SetValidator(
            new GroupsValidator<UpdateGroupRequest>(authManager, groupManager)
        );
    }
}