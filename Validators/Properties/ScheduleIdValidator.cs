using AMS.Interfaces;
using FluentValidation;
using FluentValidation.Validators;

namespace AMS.Validators;

class ScheduleIdValidator<T> : PropertyValidator<T, string>
{

    private readonly IScheduleManager scheduleManager;

    public IAuthManager authManager { get; set; }
    public ScheduleIdValidator(IScheduleManager scheduleManager, IAuthManager authManager)
    {
        this.authManager = authManager;
        this.scheduleManager = scheduleManager;
    }

    public override string Name => "ValidScheduleValidator";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        if (value == null)
        {
            return true;
        }

        return scheduleManager.CheckIfScheduleExists(value.ToString(), authManager.GetUserOrganizationId());
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "Schedule does not exist.";
    }
}