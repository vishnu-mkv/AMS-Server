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
    // group
    AddGroup,
    UpdateGroup,
    DeleteGroup,
    ListGroups,
    ReadGroup,
    // topic
    AddTopic,
    UpdateTopic,
    DeleteTopic,
    ListTopics,
    ReadTopic,

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
        { PermissionEnum.AddGroup, new AppPermission("Add Group", "Add a new group") },
        { PermissionEnum.UpdateGroup, new AppPermission("Update Group", "Update an existing group") },
        { PermissionEnum.DeleteGroup, new AppPermission("Delete Group", "Delete a group") },
        { PermissionEnum.ListGroups, new AppPermission("List Groups", "List all groups") },
        { PermissionEnum.ReadGroup, new AppPermission("Read Group", "Read a group") },
        { PermissionEnum.AddTopic, new AppPermission("Add Topic", "Add a new topic") },
        { PermissionEnum.UpdateTopic, new AppPermission("Update Topic", "Update an existing topic") },
        { PermissionEnum.DeleteTopic, new AppPermission("Delete Topic", "Delete a topic") },
        { PermissionEnum.ListTopics, new AppPermission("List Topics", "List all topics") },
        { PermissionEnum.ReadTopic, new AppPermission("Read Topic", "Read a topic") },
    };
}
