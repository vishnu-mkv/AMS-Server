using AMS.Interfaces;
using AMS.Requests;
using FluentValidation;

namespace AMS.Validators;

public class UpdateGroupValidaor : AbstractValidator<UpdateGroupRequest>
{
    public UpdateGroupValidaor(IUserManager userManager, IAuthManager authManager, IGroupManager groupManager, IScheduleManager scheduleManager)
    {

        RuleFor(x => x.Name).MinimumLength(3).WithMessage("Name must be at least 3 characters");

        RuleFor(x => x.Users).SetValidator(
            new UsersValidator<UpdateGroupRequest>(authManager, userManager)
        );

        // when group type is GroupOfGroups, groups must be provided else groups should be null
        RuleFor(x => x.Groups).SetValidator(
            new GroupsValidator<UpdateGroupRequest>(authManager, groupManager)
        );

        RuleFor(x => x.ScheduleId).SetValidator(
            new ScheduleIdValidator<UpdateGroupRequest>(scheduleManager, authManager)
        );
    }
}