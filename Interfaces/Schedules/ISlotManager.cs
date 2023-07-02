
using AMS.Models;
using AMS.Requests;

namespace AMS.Interfaces;

public interface ISlotManager
{

    public Slot? GetSlot(string ScheduleId, int Day, string TimeSlotId, bool create = false);
    public bool RemoveSlotsForDay(string ScheduleId, int Day);
    public List<Slot> EnsureSlots(string ScheduleId, SlotMap[] slotMap);

}