using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Managers;

public class AttendanceStatusProvider : IAttendanceStatusProvider
{

    // hashmaps for storing attendance status
    private Dictionary<string, AttendanceStatus> _attendanceStatuses = new();
    private readonly IServiceScopeFactory _serviceScope;

    public AttendanceStatusProvider(IServiceScopeFactory serviceScope)
    {
        _serviceScope = serviceScope;
        Initialize();
    }

    async Task Initialize()
    {
        await ExecuteWithContext(async context =>
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            // load all roles and permissions
            var attendanceStatuses = await context.AttendanceStatuses.AsNoTracking().ToListAsync();
            _attendanceStatuses = attendanceStatuses.ToDictionary(a => a.Id);
            return true;
        });
    }

    public List<AttendanceStatus?> GetAttendanceStatus(string[] attendanceStatusIds)
    {
        var attendanceStatuses = attendanceStatusIds.Select(attendanceStatusId => _attendanceStatuses.GetValueOrDefault(attendanceStatusId)).Where(a => a != null).ToList();
        return attendanceStatuses;
    }

    public AttendanceStatus? GetAttendanceStatus(string attendanceStatusId)
    {
        return GetAttendanceStatus(new string[] { attendanceStatusId }).FirstOrDefault() ?? null;
    }


    public bool AttendanceStatusExists(string attendanceStatusId)
    {
        return AttendanceStatusExists(new string[] { attendanceStatusId });
    }

    public bool AttendanceStatusExists(string[] attendanceStatusIds)
    {
        // check if all attendance statuses exist
        return attendanceStatusIds.All(_attendanceStatuses.ContainsKey);
    }

    private async Task<T> ExecuteWithContext<T>(Func<ApplicationDbContext, Task<T>> action)
    {
        if (_serviceScope == null)
            throw new Exception("Service scope is not initialized.");

        using var scope = _serviceScope.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        return await action(context);
    }

    public List<AttendanceStatus> GetAllStatus()
    {
        return _attendanceStatuses.Values.ToList();
    }
}