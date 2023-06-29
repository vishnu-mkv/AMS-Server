using AMS.Interfaces;
using AMS.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace AMS.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator(IRoleProvider rolePermissionProvider, IAuthManager authManager, IUserManager userManager,
                            IServiceScopeFactory serviceScopeFactory, IScheduleManager scheduleManager)
    {
        var max = 20;
        RuleFor(x => x.FirstName).Length(2, max).WithMessage("First name is required.");
        RuleFor(x => x.LastName).Length(1, max).WithMessage("Last name is required.");
        RuleFor(x => x.DOB).LessThan(DateTime.Today).WithMessage("Date of birth cannot be in the future.");

        RuleFor<string>(x => x.UserName).Length(2, max)
            .NotEmpty().WithMessage("Username is required.")
            .When(x =>
                // get the user id from the route
                CheckIfSignInAllowed(userManager, serviceScopeFactory, x)
            );

        RuleFor(x => x.Password).Length(2, max)
            .NotEmpty().WithMessage("Password is required.")
            .When(x => CheckIfSignInAllowed(userManager, serviceScopeFactory, x));

        RuleFor(x => x.RoleIds)
            .SetValidator(new RolesValidator<UpdateUserRequest>(rolePermissionProvider, authManager));

        RuleFor(x => x.ScheduleId)
            .SetValidator(new ScheduleIdValidator<UpdateUserRequest>(scheduleManager, authManager));
    }

    private static bool CheckIfSignInAllowed(IUserManager userManager, IServiceScopeFactory scopeFactory, UpdateUserRequest x)
    {
        using var scope = scopeFactory.CreateScope();
        var actionContext = scope.ServiceProvider.GetRequiredService<IActionContextAccessor>();

        var routeValues = actionContext.ActionContext.RouteData.Values;
        var id = routeValues["id"]?.ToString();

        if (id == null) return false;

        var user = userManager.GetUserById(id);
        return (x.SignInAllowed == true || user?.SignInAllowed == true) && user.UserName == null;
    }

}