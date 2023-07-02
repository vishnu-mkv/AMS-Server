namespace AMS.Models;


public class TimeSlot
{
    public string Id { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public string? ScheduleId { get; set; }

    public virtual Schedule? Schedule { get; set; }

    public TimeSlot()
    {
        Id = Guid.NewGuid().ToString();
    }
}