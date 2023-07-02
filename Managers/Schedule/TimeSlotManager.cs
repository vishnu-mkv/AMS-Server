using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using AMS.Requests;

namespace AMS.Managers;

public class TimeSlotManager : ITimeSlotManager

{
    private readonly IScheduleManager scheduleManager;
    private readonly ApplicationDbContext context;

    public TimeSlotManager(IScheduleManager scheduleManager, ApplicationDbContext context)
    {
        this.scheduleManager = scheduleManager;
        this.context = context;
    }

    public TimeSlot AddTimeSlot(AddTimeSlotRequest addTimeSlotRequest, string scheduleId)
    {
        if (!scheduleManager.CheckIfScheduleExists(scheduleId)) throw new Exception("Schedule not found.");

        var timeSlot = new TimeSlot
        {
            ScheduleId = scheduleId,
            StartTime = new TimeSpan(addTimeSlotRequest.StartHour, addTimeSlotRequest.StartMinute, 0),
            EndTime = new TimeSpan(addTimeSlotRequest.EndHour, addTimeSlotRequest.EndMinute, 0),
        };

        context.TimeSlots.Add(timeSlot);
        context.SaveChanges();
        return timeSlot;
    }

    public bool CheckIfTimeSlotExists(string id, string scheduleId)
    {
        return CheckIfTimeSlotsExist(new string[] { id }, scheduleId);
    }

    public bool CheckIfTimeSlotsExist(string[] ids, string scheduleId)
    {
        // check if all time slots exist
        return context.TimeSlots.Count(x => ids.Contains(x.Id) && x.ScheduleId == scheduleId) == ids.Length;
    }

    public bool DeleteTimeSlot(string id)
    {
        var timeSlot = GetTimeSlot(id) ?? throw new Exception("Time slot not found.");
        context.TimeSlots.Remove(timeSlot);
        context.SaveChanges();
        return true;
    }

    public TimeSlot? GetTimeSlot(string id)
    {
        return context.TimeSlots.FirstOrDefault(x => x.Id == id);
    }

    public List<TimeSlot> GetTimeSlots(string scheduleId)
    {
        return context.TimeSlots.Where(x => x.ScheduleId == scheduleId).ToList();
    }

    public TimeSlot UpdateTimeSlot(string id, AddTimeSlotRequest updateTimeSlotRequest)
    {
        var timeSlot = GetTimeSlot(id) ?? throw new Exception("Time slot not found.");

        timeSlot.StartTime = new TimeSpan(updateTimeSlotRequest.StartHour, updateTimeSlotRequest.StartMinute, 0);
        timeSlot.EndTime = new TimeSpan(updateTimeSlotRequest.EndHour, updateTimeSlotRequest.EndMinute, 0);

        context.SaveChanges();

        return timeSlot;
    }
}