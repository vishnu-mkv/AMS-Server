using AMS.Data;
using AMS.Interfaces;
using AMS.Managers.Auth;
using AMS.Models;
using AMS.Requests;

namespace AMS.Managers;

public class ScheduleManager : IScheduleManager
{

    private readonly ApplicationDbContext _context;
    private readonly IAuthManager _authManager;
    public ScheduleManager(ApplicationDbContext context, IAuthManager authManager)
    {
        _context = context;
        _authManager = authManager;
    }

    public Schedule AddSchedule(AddScheduleRequest addScheduleRequest)
    {
        var schedule = new Schedule
        {
            Name = addScheduleRequest.Name,
            OrganizationId = _authManager.GetUserOrganizationId()
        };

        _context.Schedules.Add(schedule);
        _context.SaveChanges();

        return schedule;
    }

    public bool CheckIfScheduleExists(string id)
    {
        return _context.Schedules.Any(x => x.Id == id);
    }

    public Schedule? GetSchedule(string id)
    {
        return _context.Schedules.Find(id);
    }

    // list all schedules
    public List<Schedule> GetSchedules()
    {
        return _context.Schedules.Where(s => s.OrganizationId == _authManager.GetUserOrganizationId()).ToList();
    }
}