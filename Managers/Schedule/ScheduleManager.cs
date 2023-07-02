using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using AMS.Requests;
using Microsoft.EntityFrameworkCore;

namespace AMS.Managers;

public class ScheduleManager : IScheduleManager
{

    private readonly ApplicationDbContext _context;
    private readonly IAuthManager _authManager;
    private readonly ISessionManager _sessionManager;

    public ScheduleManager(ApplicationDbContext context, IAuthManager authManager, ISessionManager sessionManager)
    {
        _context = context;
        _authManager = authManager;
        _sessionManager = sessionManager;
    }

    public Schedule AddSchedule(AddScheduleRequest addScheduleRequest)
    {
        var schedule = new Schedule
        {
            Name = addScheduleRequest.Name,
            OrganizationId = _authManager.GetUserOrganizationId(),
            CreatedById = _authManager.GetCurrentUserId(),
            Color = addScheduleRequest.Color ?? "#ccc"
        };

        if (addScheduleRequest.Days.Length > 0)
        {
            schedule.Days = addScheduleRequest.Days;
        }

        _context.Schedules.Add(schedule);
        _context.SaveChanges();

        return schedule;
    }

    // update schedule
    public Schedule UpdateSchedule(string id, UpdateScheduleRequest updateScheduleRequest)
    {
        var schedule = GetSchedule(id) ?? throw new Exception("Schedule not found.");

        var oldDays = schedule.Days;

        schedule.Name = updateScheduleRequest.Name ?? schedule.Name;
        schedule.Days = updateScheduleRequest.Days ?? schedule.Days;
        schedule.Color = updateScheduleRequest.Color ?? schedule.Color;

        if (updateScheduleRequest.Days != null && oldDays != null)
        {
            var removedDays = oldDays.Except(updateScheduleRequest.Days).ToList();
            foreach (var day in removedDays)
            {
                _sessionManager.CleanSessionsForDay(schedule.Id, day);
            }
        }

        _context.SaveChanges();

        return schedule;
    }

    public bool CheckIfScheduleExists(string id, string organizationId)
    {
        return _context.Schedules.Any(x => x.Id == id && x.OrganizationId == organizationId);
    }

    public Schedule? GetSchedule(string id, bool populate = false)
    {
        var schedule = _context.Schedules.FirstOrDefault(x => x.Id == id && x.OrganizationId == _authManager.GetUserOrganizationId());
        if (schedule == null) return null;

        if (populate)
        {
            schedule.TimeSlots = _context.TimeSlots.Where(x => x.ScheduleId == schedule.Id).ToList();
            schedule.Sessions = _context.Sessions.Where(x => x.ScheduleId == schedule.Id).Include(x => x.Slots).ToList();
        }

        return schedule;
    }


    // list all schedules
    public List<Schedule> GetSchedules()
    {
        return _context.Schedules.Where(s => s.OrganizationId == _authManager.GetUserOrganizationId()).ToList();
    }

    public bool CheckIfScheduleExists(string id)
    {
        return CheckIfScheduleExists(id, _authManager.GetUserOrganizationId());
    }
}