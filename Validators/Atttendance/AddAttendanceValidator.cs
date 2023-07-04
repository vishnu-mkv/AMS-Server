using AMS.Interfaces;
using AMS.Managers;
using AMS.Requests;
using Azure.Core;
using FluentValidation;

namespace AMS.Validators;

public class AddAttendanceValidator : AbstractValidator<AddAttendanceRequest>
{
    public AddAttendanceValidator(IScheduleManager scheduleManager, IAuthManager authManager, ISessionManager sessionManager,
    IGroupManager groupManager, IUserManager userManager, IAttendanceStatusProvider attendanceStatusManager,
    ITimeSlotManager timeSlotManager, ISlotManager SlotManager)
    {
        RuleFor(x => x.ScheduleId).NotEmpty().SetValidator(
            new ScheduleIdValidator<AddAttendanceRequest>(scheduleManager, authManager)
        );

        RuleFor(x => x.SessionId).NotEmpty().Must((request, sessionId) =>
             sessionManager.CheckIfSessionExists(sessionId, request.ScheduleId)
        ).WithMessage("Session does not exist in the schedule");

        RuleFor(x => x.GroupId).NotEmpty().Must((request, groupId) =>
            groupManager.CheckGroupExists(groupId)
        ).WithMessage("Group does not exist");

        RuleFor(x => x.AttendanceEntries).NotEmpty().Must((request, attendanceEntries) =>
        {
            var userIdsAll = attendanceEntries.Select(x => x.UserId).ToArray();
            var userIds = userIdsAll.Distinct().ToArray();

            return userIds.Length == userIds.Length && userIds.All(
                x => userManager.UserExists(authManager.GetUserOrganizationId(), x)
            ) && groupManager.CheckIfUsersBelongToGroup(request.GroupId, userIds);

        }).WithMessage("One or more users do not exist");

        RuleFor(x => x.AttendanceEntries).Must((Request, attendanceEntries) =>
        {
            var statuses = attendanceEntries.Select(x => x.AttendanceStatusId).Distinct().ToArray();
            return statuses.All(x => attendanceStatusManager.AttendanceStatusExists(x));
        }).WithMessage("One or more attendance statuses do not exist");

        RuleFor(x => x.GroupAccessPath).NotEmpty().SetValidator(
            new GroupsValidator<AddAttendanceRequest>(authManager, groupManager)
        );

        RuleFor(x => x.TimeSlotId).NotEmpty().Must((request, timeSlotId) =>
            timeSlotManager.CheckIfTimeSlotExists(timeSlotId, request.ScheduleId)
        ).WithMessage("Time slot does not exist in the schedule");

        RuleFor(x => x.Date).NotEmpty().Must((request, date) =>
            {
                int Day = (int)date.DayOfWeek;
                var slot = SlotManager.GetSlot(request.ScheduleId, Day, request.TimeSlotId);
                return slot != null;
            }
        ).WithMessage("Session does not exist for the given date and time slot")
        .Must(x => x.Date.Date <= DateTime.UtcNow.Date).WithMessage("Date cannot be in the future");
    }
}