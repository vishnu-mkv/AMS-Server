using AMS.Requests;
using FluentValidation;

namespace AMS.Validators;

public class AddTimeSlotValidator : AbstractValidator<AddTimeSlotRequest>
{
    public AddTimeSlotValidator()
    {
        RuleFor(x => x.StartHour)
            .InclusiveBetween(0, 23).WithMessage("Start hour must be between 0 and 23.");

        RuleFor(x => x.StartMinute)
            .InclusiveBetween(0, 59).WithMessage("Start minute must be between 0 and 59.");

        RuleFor(x => x.EndHour)
            .InclusiveBetween(0, 23).WithMessage("End hour must be between 0 and 23.");

        RuleFor(x => x.EndMinute)
            .InclusiveBetween(0, 59).WithMessage("End minute must be between 0 and 59.");

        RuleFor(x => x.StartHour)
            .LessThan(x => x.EndHour).When(x => x.StartHour == x.EndHour && x.StartMinute > x.EndMinute)
            .WithMessage("Start time must be before end time.");

        RuleFor(x => x.StartHour)
            .LessThan(x => x.EndHour).When(x => x.StartHour > x.EndHour)
            .WithMessage("Start time must be before end time.");

        RuleFor(x => x.StartHour)
            .LessThan(x => x.EndHour).When(x => x.StartHour == x.EndHour && x.StartMinute == x.EndMinute)
            .WithMessage("Start time must be before end time.");
    }
}