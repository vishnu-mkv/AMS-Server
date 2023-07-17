using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Data.Seeders;

public class AttendanceStatusSeeder
{

    private readonly IServiceProvider _services;

    private readonly List<AttendanceStatus> _attendanceStatusList = new(){
        new AttendanceStatus{
            Id = "1",
            Color = "#aaa",
             Name = "Present",
             ShortName = "P"
        },
        new AttendanceStatus{
            Id = "2",
            Color = "#eee",
             Name = "Absent",
             ShortName = "A"
        },
        new AttendanceStatus{
            Id = "3",
            Color = "#fff",
             Name = "On-Duty",
             ShortName = "D"
        },
     };

    public AttendanceStatusSeeder(IServiceProvider services)
    {
        _services = services;
    }

    public async void Seed()
    {
        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        // check if default organization exists
        var Organizationname = "KEC";

        var organization = context.Organizations.FirstOrDefault(organization => organization.Name == Organizationname);

        if (organization == null) throw new Exception("Default Organization does not exist");

        // check if attendance status exists using db context
        var attendanceIds = _attendanceStatusList.Select(a => a.Id).ToList();
        var attendanceStatuses = context.AttendanceStatuses.Where(a => attendanceIds.Contains(a.Id)).ToList();

        var notPresentAttendanceStatuses = attendanceIds.Except(attendanceStatuses.Select(a => a.Id)).ToList();

        // create attendance status if it does not exist
        var newAttendanceStatus = _attendanceStatusList.Where(a => notPresentAttendanceStatuses.Contains(a.Id)).Select(a => new AttendanceStatus
        {
            Id = a.Id,
            Name = a.Name,
            ShortName = a.ShortName,
            Color = a.Color,
            OrganizationId = organization.Id
        }).ToList();

        if (newAttendanceStatus.Count > 0)
        {
            await context.AttendanceStatuses.AddRangeAsync(newAttendanceStatus);
            await context.SaveChangesAsync();
        }
    }
}