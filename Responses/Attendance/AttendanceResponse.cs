namespace AMS.Responses;

public class AttendanceResponse
{

    public string Id { get; set; }

    public ScheduleResponse Schedule { get; set; }

    public DateTime RecordedFor { get; set; }

    public DateTime? Created { get; } = DateTime.Now;

    public SessionSummaryResponse Session { get; set; }

    public GroupSummaryResponse Group { get; set; }

    public SlotResponse Slot { get; set; }

    public ICollection<RecordResponse> Records { get; set; } = new List<RecordResponse>();
}

public class AttendanceSummaryResponse
{
    public string Id { get; set; }

    public string ScheduleId { get; set; }

    public DateTime RecordedFor { get; set; }

    public DateTime? Created { get; } = DateTime.Now;

    public string SessionId { get; set; }

    public GroupSummaryResponse Group { get; set; }

    public SlotResponse Slot { get; set; }
}

public class AttendanceResponseWithoutRecords
{
    public string Id { get; set; }

    public ScheduleResponse Schedule { get; set; }

    public DateTime RecordedFor { get; set; }

    public DateTime? Created { get; } = DateTime.Now;

    public SessionSummaryResponse Session { get; set; }

    public GroupSummaryResponse Group { get; set; }

    public SlotResponse Slot { get; set; }
}