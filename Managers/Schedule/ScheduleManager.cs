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
    private readonly IServiceProvider _serviceProvider;

    public ScheduleManager(ApplicationDbContext context, IAuthManager authManager, ISessionManager sessionManager, IServiceProvider serviceProvider)
    {
        _context = context;
        _authManager = authManager;
        _sessionManager = sessionManager;
        _serviceProvider = serviceProvider;
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

        if (addScheduleRequest.TimeSlots?.Length > 0)
        {
            var _timeSlotManager = (ITimeSlotManager)_serviceProvider.GetService(typeof(ITimeSlotManager));

            foreach (var timeSlot in addScheduleRequest.TimeSlots)
            {
                _timeSlotManager.AddTimeSlot(timeSlot, schedule.Id);
            }
        }

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
        var schedules = _context.Schedules.AsQueryable();

        if (populate)
        {
            schedules = schedules.Include(s => s.Sessions).ThenInclude(s => s.Slots)
                .Include(s => s.Sessions).ThenInclude(s => s.Groups).Include(s => s.Sessions).ThenInclude(s => s.AttendanceTakers)
                .Include(s => s.Sessions).ThenInclude(s => s.Topic);
        }

        var schedule = schedules.Include(s => s.TimeSlots).FirstOrDefault(s => s.Id == id && s.OrganizationId == _authManager.GetUserOrganizationId());

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