using AMS.Interfaces;
using FluentValidation;
using FluentValidation.Validators;

namespace AMS.Validators;

class UsersValidator<T> : PropertyValidator<T, string[]>
{
    private readonly IAuthManager authManager;
    private readonly IUserManager userManager;

    public UsersValidator(IAuthManager authManager, IUserManager userManager)
    {
        this.authManager = authManager;
        this.userManager = userManager;
    }

    public override string Name => "ValidUsersValidator";

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "User does not exist.";
    }

    public override bool IsValid(ValidationContext<T> context, string[] value)
    {
        if (value == null)
        {
            return true;
        }

        string organizationId = authManager.GetUserOrganization().Id;
        return value.All(u => userManager.UserExists(organizationId, u));

    }
}