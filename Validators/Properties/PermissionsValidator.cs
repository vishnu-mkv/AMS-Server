using AMS.Interfaces;
using FluentValidation;
using FluentValidation.Validators;

namespace AMS.Validators;

class PermissionsValidator<T> : PropertyValidator<T, string[]>
{
    private readonly IRoleProvider roleProvider;

    public PermissionsValidator(IRoleProvider roleProvider)
    {
        this.roleProvider = roleProvider;
    }

    public override string Name => "ValidPermissionsValidator";

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "Permission does not exist.";
    }

    public override bool IsValid(ValidationContext<T> context, string[] value)
    {
        if (value == null)
        {
            return true;
        }

        if (value.Length == 0)
        {
            return false;
        }

        return value.All(p => roleProvider.Permissions.ContainsKey(p));

    }
}