using System.ComponentModel.DataAnnotations;

namespace AMS.Requests;

public class AddAttendanceRequest
{
    public string? ScheduleId { get; set; }

    public string? SessionId { get; set; }

    public string? GroupId { get; set; }

    public string? TimeSlotId { get; set; }

    public string[] GroupAccessPath { get; set; } = Array.Empty<string>();

    public List<AttendanceEntry> AttendanceEntries { get; set; } = new List<AttendanceEntry>();

    [Required]
    public DateTime Date { get; set; }
}

public class AttendanceEntry
{
    public string UserId { get; set; }

    public string AttendanceStatusId { get; set; }
}
