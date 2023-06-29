using AMS.Interfaces;
using FluentValidation;
using FluentValidation.Validators;

namespace AMS.Validators;

public class RolesValidator<T> : PropertyValidator<T, string[]>
{
    private readonly IRoleProvider rolePermissionProvider;
    private readonly IAuthManager authManager;

    public RolesValidator(IRoleProvider rolePermissionProvider, IAuthManager authManager)
    {
        this.rolePermissionProvider = rolePermissionProvider;
        this.authManager = authManager;
    }

    public override string Name => "ValidRolesValidator";

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "Role does not exist.";
    }

    public override bool IsValid(ValidationContext<T> context, string[] value)
    {
        if (value == null)
        {
            return true;
        }


        return rolePermissionProvider.CheckRolesExists(value.ToArray()) &&
               rolePermissionProvider.GetRolesByIds(value).TrueForAll(r =>
               {
                   string id = authManager.GetUserOrganization().Id;
                   return r.Organization.Id == id;
               });

    }
}