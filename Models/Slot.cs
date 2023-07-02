namespace AMS.Models;

public class Slot
{
    public string Id { get; set; }

    public TimeSlot TimeSlot { get; set; }

    public string TimeSlotId { get; set; }

    public int Day { get; set; }

    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    public string? ScheduleId { get; set; }

    public virtual Schedule? Schedule { get; set; }

    public Slot()
    {
        Id = Guid.NewGuid().ToString();
    }
}