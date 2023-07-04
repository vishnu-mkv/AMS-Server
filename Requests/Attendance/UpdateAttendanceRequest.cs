namespace AMS.Requests;

public class UpdateAttendanceRequest
{
    public string AttendanceId { get; set; }
    public string[] GroupAccessPath { get; set; } = Array.Empty<string>();
    public List<AttendanceEntry> AttendanceEntries { get; set; } = new List<AttendanceEntry>();
}