using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using AMS.Requests;

namespace AMS.Managers;

public class SlotManager : ISlotManager
{
    private readonly ApplicationDbContext context;

    public SlotManager(ApplicationDbContext context)
    {
        this.context = context;
    }

    public Slot? GetSlot(string ScheduleId, int Day, string TimeSlotId, bool create = false)
    {
        Slot? slot = context.Slots.FirstOrDefault(x => x.ScheduleId == ScheduleId && x.Day == Day && x.TimeSlotId == TimeSlotId);

        if (slot == null && create)
        {
            slot = new Slot
            {
                ScheduleId = ScheduleId,
                Day = Day,
                TimeSlotId = TimeSlotId
            };

            context.Slots.Add(slot);
            context.SaveChanges();
        }
        return slot;
    }

    public bool RemoveSlotsForDay(string ScheduleId, int Day)
    {
        var slots = context.Slots.Where(x => x.ScheduleId == ScheduleId && x.Day == Day);
        context.Slots.RemoveRange(slots);
        context.SaveChanges();
        return true;
    }

    public List<Slot> EnsureSlots(string ScheduleId, SlotMap[] slotMap)
    {

        // get or create slots
        List<Slot> slots = new();

        foreach (var slot in slotMap)
        {
            foreach (var timeSlotId in slot.TimeSlotIds)
            {
                var slotObj = GetSlot(ScheduleId, slot.Day, timeSlotId, true);
                if (slotObj != null) slots.Add(slotObj);
            }
        }

        return slots;
    }
}