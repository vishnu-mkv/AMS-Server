using AMS.Interfaces;
using AMS.Requests;
using FluentValidation;

namespace AMS.Validators;

public class UpdateAttendanceValidator : AbstractValidator<UpdateAttendanceRequest>
{

    public UpdateAttendanceValidator(IAuthManager authManager, IGroupManager groupManager,
                                    IUserManager userManager, IAttendanceStatusProvider attendanceStatusManager,
                                    IAttendanceManager attendanceManager)
    {

        RuleFor(x => x.AttendanceId).NotEmpty().Must((request, attendanceId) =>
            attendanceManager.AttendanceExists(attendanceId)
        ).WithMessage("Attendance does not exist");

        RuleFor(x => x.AttendanceEntries).NotEmpty().Must((request, attendanceEntries) =>
       {
           var userIdsAll = attendanceEntries.Select(x => x.UserId).ToArray();
           var userIds = userIdsAll.Distinct().ToArray();
           var GroupId = attendanceManager.GetAttendance(request.AttendanceId).GroupId;

           return userIds.Length == userIds.Length && userIds.All(
               x => userManager.UserExists(authManager.GetUserOrganizationId(), x)
           ) && groupManager.CheckIfUsersBelongToGroup(GroupId, userIds);

       }).WithMessage("One or more users do not exist");

        RuleFor(x => x.AttendanceEntries).Must((Request, attendanceEntries) =>
        {
            var statuses = attendanceEntries.Select(x => x.AttendanceStatusId).Distinct().ToArray();
            return statuses.All(x => attendanceStatusManager.AttendanceStatusExists(x));
        }).WithMessage("One or more attendance statuses do not exist");

        RuleFor(x => x.GroupAccessPath).NotEmpty().SetValidator(
            new GroupsValidator<UpdateAttendanceRequest>(authManager, groupManager)
        );

    }
}