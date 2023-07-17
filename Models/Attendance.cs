namespace AMS.Models;

public class Attendance
{

    public string Id { get; set; }

    public string? ScheduleId { get; set; }

    public Schedule? Schedule { get; set; }

    public DateTime RecordedFor { get; set; }

    public DateTime? Created { get; set; } = DateTime.Now;

    public string SessionId { get; set; }

    public Session? Session { get; set; }

    public string? TopicId { get; set; }

    public Topic? Topic { get; set; }

    public string? GroupId { get; set; }

    public Group? Group { get; set; }

    public string? SlotId { get; set; }

    public Slot? Slot { get; set; }

    public ICollection<Record> Records { get; set; } = new List<Record>();

    public Organization? Organization { get; set; }
    public string? OrganizationId { get; set; } = "default";

    public Attendance()
    {
        Id = Guid.NewGuid().ToString();
    }
}