using AMS.Interfaces;
using AMS.Requests;
using FluentValidation;

namespace AMS.Validators;

public class AddSessionValidator : AbstractValidator<AddSessionRequest>
{
    public AddSessionValidator(ITimeSlotManager timeSlotManager, ITopicManager topicManager,
                                IAuthManager authManager, IGroupManager groupManager, IUserManager userManager,
                                IScheduleManager scheduleManager)
    {
        RuleFor(x => x.ScheduleId)
            .SetValidator(new ScheduleIdValidator<AddSessionRequest>(scheduleManager, authManager
            ));
        RuleFor(x => x.TopicId)
            .SetValidator(new TopicIdValidator<AddSessionRequest>(authManager, topicManager));

        RuleFor(x => x.Slots)
            .NotEmpty()
            .Must(slots =>
            {
                var days = slots.Select(x => x.Day).Distinct().ToList();
                return days.Count == slots.Length;
            })
            .WithMessage("Duplicate day")
            .Must(slots =>
            {
                foreach (var slot in slots)
                {
                    var timeSlots = slot.TimeSlotIds.Distinct().ToList();
                    if (timeSlots.Count != slot.TimeSlotIds.Length)
                    {
                        return false;
                    }
                }
                return true;
            })
            .WithMessage("Duplicate time slot")
            .Must((request, slots) =>
            {
                string[] timeSlots = slots.SelectMany(x => x.TimeSlotIds).Distinct().ToArray();
                return timeSlotManager.CheckIfTimeSlotsExist(timeSlots, request.ScheduleId);
            })
            .WithMessage("Time slot does not exist");

        RuleFor(x => x.AttendanceTakerIds)
            .NotEmpty()
            .SetValidator(new UsersValidator<AddSessionRequest>(authManager, userManager));

        RuleFor(x => x.GroupIds)
            .NotEmpty()
            .SetValidator(new GroupsValidator<AddSessionRequest>(authManager, groupManager));
    }
}