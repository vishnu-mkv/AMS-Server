using AMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace AMS.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public Permission[] Permissions { get; }

        public PermissionRequirement(Permission[] permissions)
        {
            Permissions = permissions;
        }
    }
}