using AMS.Providers;
using Microsoft.AspNetCore.Authorization;

namespace AMS.Authorization
{

    public class HasPermissionAttribute : AuthorizeAttribute
    {

        public HasPermissionAttribute(PermissionEnum[] permissions)
            : base(permissions.Select(p => p.ToString()).Aggregate((a, b) => $"{a},{b}"))
        {
        }

        public HasPermissionAttribute(PermissionEnum permission)
            : base(permission.ToString())
        {
        }
    }
}