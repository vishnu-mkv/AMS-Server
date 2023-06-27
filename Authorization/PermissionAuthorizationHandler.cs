using System.Security.Claims;
using AMS.Interfaces;
using AMS.Models;
using AMS.Providers;
using Microsoft.AspNetCore.Authorization;

namespace AMS.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{

    private readonly IServiceScopeFactory _scopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        string[] roles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();

        // if no roles, then no access
        if (roles.Length == 0)
        {
            return;
        }

        using IServiceScope scope = _scopeFactory.CreateScope();

        IRoleProvider roleProvider = scope.ServiceProvider.GetRequiredService<IRoleProvider>();

        var AdminPermission = new Permission(PermissionEnum.AdminAccess);

        var userPermissions = roleProvider.GetPermissions(roles).Select(p => p.Id);

        // get all permissions for the roles
        bool authorized = userPermissions.Any(p => p == AdminPermission.Id) || requirement.Permissions.All(p => userPermissions.Contains(p.Id));

        if (!authorized)
        {
            return;
        }

        context.Succeed(requirement);
    }
}