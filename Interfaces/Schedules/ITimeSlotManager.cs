using AMS.Models;
using AMS.Requests;

namespace AMS.Interfaces;

public interface ITimeSlotManager
{

    public TimeSlot AddTimeSlot(AddTimeSlotRequest addTimeSlotRequest, string scheduleId);
    public TimeSlot UpdateTimeSlot(string id, AddTimeSlotRequest updateTimeSlotRequest);
    public bool CheckIfTimeSlotExists(string id, string scheduleId);
    public bool CheckIfTimeSlotsExist(string[] ids, string scheduleId);
    public TimeSlot? GetTimeSlot(string id);
    public List<TimeSlot> GetTimeSlots(string scheduleId);
    public bool DeleteTimeSlot(string id);
}