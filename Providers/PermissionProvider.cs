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
    // time slot
    AddTimeSlot,
    UpdateTimeSlot,
    DeleteTimeSlot,
    ListTimeslots,
    // add session
    AddSession,
    UpdateSession,
    DeleteSession,
    ListSessions,
    ReadSession,
    // add attendance
    GetAttendance,
    ListAttendances,
    AddAttendance,
    UpdateAttendance,
    ReadAttendance,
    DeleteAttendance,
    ListAttendanceStatuses

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
        { PermissionEnum.AddTimeSlot, new AppPermission("Add Time Slot", "Add a new time slot") },
        { PermissionEnum.DeleteTimeSlot, new AppPermission("Delete Time Slot", "Delete a time slot") },
        { PermissionEnum.UpdateTimeSlot, new AppPermission("Update Time Slot", "Update an existing time slot") },
        { PermissionEnum.ListTimeslots, new AppPermission("List Time Slots", "List all time slots") },
        { PermissionEnum.AddSession, new AppPermission("Add Session", "Add a new session") },
        { PermissionEnum.UpdateSession, new AppPermission("Update Session", "Update an existing session") },
        { PermissionEnum.DeleteSession, new AppPermission("Delete Session", "Delete a session") },
        { PermissionEnum.ListSessions, new AppPermission("List Sessions", "List all sessions") },
        { PermissionEnum.ReadSession, new AppPermission("Read Session", "Read a session") },
        { PermissionEnum.GetAttendance, new AppPermission("Get Attendance", "Get attendance") },
        { PermissionEnum.ListAttendances, new AppPermission("List Attendances", "List all attendances") },
        { PermissionEnum.AddAttendance, new AppPermission("Add Attendance", "Add a new attendance") },
        { PermissionEnum.UpdateAttendance, new AppPermission("Update Attendance", "Update an existing attendance") },
        { PermissionEnum.ReadAttendance, new AppPermission("Read Attendance", "Read an attendance") },
        { PermissionEnum.DeleteAttendance, new AppPermission("Delete Attendance", "Delete an attendance") },
        { PermissionEnum.ListAttendanceStatuses, new AppPermission("List Attendance Statuses", "List all attendance statuses") }

    };
}
