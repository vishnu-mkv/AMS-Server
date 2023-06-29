using AMS.Interfaces;
using FluentValidation;
using FluentValidation.Validators;

namespace AMS.Validators;

class GroupsValidator<T> : PropertyValidator<T, string[]>
{
    private readonly IAuthManager authManager;

    public IGroupManager GroupManager { get; }
    public GroupsValidator(IAuthManager authManager, IGroupManager groupManager)
    {
        GroupManager = groupManager;
        this.authManager = authManager;
    }

    public override string Name => "ValidGroupsValidator";

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "Group does not exist.";
    }

    public override bool IsValid(ValidationContext<T> context, string[] value)
    {
        if (value == null)
        {
            return true;
        }

        string organizationId = authManager.GetUserOrganization().Id;
        return value.All(u => GroupManager.GroupExists(organizationId, u));

    }
}