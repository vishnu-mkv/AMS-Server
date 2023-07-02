namespace AMS.Requests;

public class AddSessionRequest
{
    public string ScheduleId { get; set; }
    public string? TopicId { get; set; }

    public SlotMap[] Slots { get; set; }

    public string[] AttendanceTakerIds { get; set; }

    public string[] GroupIds { get; set; }
}

public class SlotMap
{
    public int Day { get; set; }
    public string[] TimeSlotIds { get; set; }
}