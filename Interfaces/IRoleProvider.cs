using AMS.Models;
using AMS.Providers;

namespace AMS.Interfaces
{
    public interface IRoleProvider
    {

        Dictionary<string, Permission> Permissions { get; }

        bool CheckForPermission(string[] roleIds, Permission permission);

        bool CheckRolesExists(string[] roleIds);
        IEnumerable<Permission> GetPermissions(string[] roleIds);
        List<Permission> GetPermissionsById(PermissionEnum[] permissions);
        List<Role> GetRolesByIds(string[] roleId);

        Task<Role> CreateRole(string name, string description, List<Permission> permissions, Organization organization);

        void AddRole(Role role);
        void RemoveRole(string id);
    }
}