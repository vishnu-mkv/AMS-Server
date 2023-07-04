namespace AMS.Models;

public class Session
{
    public string Id { get; set; }

    public Topic? Topic { get; set; }

    public string? TopicId { get; set; }

    public ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public string? ScheduleId { get; set; }

    public virtual Schedule? Schedule { get; set; }

    public ICollection<Group> Groups { get; set; } = new List<Group>();

    public ICollection<ApplicationUser> AttendanceTakers { get; set; } = new List<ApplicationUser>();

    public Session()
    {
        Id = Guid.NewGuid().ToString();
    }
}