using AMS.Requests;
using FluentValidation;

namespace AMS.Validators;

public class AddScheduleValidator : AbstractValidator<AddScheduleRequest>
{
    public AddScheduleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Schedule name must not be empty.")
            .MaximumLength(50).WithMessage("Schedule name must not exceed 50 characters.");

        RuleFor(x => x.Days)
            .Must(days => days.All(day => day >= 0 && day <= 6) && new HashSet<int>(days).Count == days.Length)
            .WithMessage("Days must be between 0 and 6.");
    }
}