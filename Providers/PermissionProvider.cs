namespace AMS.Providers;

public enum PermissionEnum
{
    AdminAccess,
    ListPermissions,
    AddUser,
    UpdateUser,
    DeleteUser,
    ListUsers,
    AddRole,
    UpdateRole,
    DeleteRole,
    ListRoles,
    AddSchedule,
    UpdateSchedule,
    DeleteSchedule,
    ListSchedules,

    // Define other permissions here
}

public class AppPermission
{
    public string Name { get; }
    public string Description { get; }

    public AppPermission(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

public static class PermissionRepo
{
    public static readonly Dictionary<PermissionEnum, AppPermission> PermissionMap = new()
    {
        { PermissionEnum.AdminAccess, new AppPermission("Admin Access", "Administrator access permission") },
        { PermissionEnum.ListPermissions, new AppPermission("List Permissions", "List all permissions") },
        { PermissionEnum.AddUser, new AppPermission("Add User", "Add a new user") },
        { PermissionEnum.UpdateUser, new AppPermission("Update User", "Update an existing user") },
        { PermissionEnum.DeleteUser, new AppPermission("Delete User", "Delete a user") },
        { PermissionEnum.ListUsers, new AppPermission("List Users", "List all users") },
        { PermissionEnum.AddRole, new AppPermission("Add Role", "Add a new role") },
        { PermissionEnum.UpdateRole, new AppPermission("Update Role", "Update an existing role") },
        { PermissionEnum.DeleteRole, new AppPermission("Delete Role", "Delete a role") },
        { PermissionEnum.ListRoles, new AppPermission("List Roles", "List all roles") },
        { PermissionEnum.AddSchedule, new AppPermission("Add Schedule", "Add a new schedule") },
        { PermissionEnum.UpdateSchedule, new AppPermission("Update Schedule", "Update an existing schedule") },
        { PermissionEnum.DeleteSchedule, new AppPermission("Delete Schedule", "Delete a schedule") },
        { PermissionEnum.ListSchedules, new AppPermission("List Schedules", "List all schedules") },
    };
}
