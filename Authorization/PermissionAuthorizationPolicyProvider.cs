using AMS.Interfaces;
using AMS.Models;
using AMS.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AMS.Authorization;

public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly IRoleProvider rolePermissionProvider;
    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, IRoleProvider rolePermissionProvider) : base(options)
    {
        this.rolePermissionProvider = rolePermissionProvider;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {

        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

        if (policy is not null)
        {
            return policy;
        }

        // policy name split by comma and convert to PermissionEnum[]

        var permissionEnums = policyName.Split(",").Select(p => (PermissionEnum)Enum.Parse(typeof(PermissionEnum), p)).ToArray();

        Permission[] permissions = rolePermissionProvider.GetPermissionsById(permissionEnums).ToArray();

        return new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(permissions))
            .Build();
    }
}
