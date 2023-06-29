using AMS.Interfaces;
using AMS.Models;
using AMS.Requests;
using FluentValidation;

namespace AMS.Validators;

public class AddGroupValidaor : AbstractValidator<AddGroupRequest>
{
    public AddGroupValidaor(IUserManager userManager, IAuthManager authManager, IGroupManager groupManager, IScheduleManager scheduleManager)
    {
        RuleFor(x => x.Id).Must(id => !groupManager.CheckGroupExists(id)).WithMessage("Group already exists");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.GroupType).IsInEnum().WithMessage("GroupType is required");
        // when group type is GroupOfUsers, users must be provided else users should be null

        RuleFor(x => x.ScheduleId).NotEmpty().WithMessage("ScheduleId is required").SetValidator(
            new ScheduleIdValidator<AddGroupRequest>(scheduleManager, authManager)
        );

        RuleFor(x => x.Users).Must(users => users == null || users.Length == 0)
        .When(x => x.ScheduleId == null).SetValidator(
            new UsersValidator<AddGroupRequest>(authManager, userManager)
        ).When(x => x.GroupType == GroupType.GroupOfUsers);

        // when group type is GroupOfGroups, groups must be provided else groups should be null
        RuleFor(x => x.Groups).Must(groups => groups == null || groups.Length == 0)
        .When(x => x.ScheduleId == null).SetValidator(
            new GroupsValidator<AddGroupRequest>(authManager, groupManager)
        ).When(x => x.GroupType == GroupType.GroupOfGroups);
    }
}