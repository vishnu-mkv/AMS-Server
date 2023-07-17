using AMS.Models;

namespace AMS.Responses;

public class GroupReportView
{
    public GroupResponse Group { get; set; } = new();

    public List<DayReportView> DayReports { get; set; } = new();

    public List<TimeSlotResponse> TimeSlots { get; set; } = new();

    public List<AttendanceStatus> AttendanceStatuses { get; set; } = new();
}

public class DayReportView
{
    public DateTime Date { get; set; }

    public List<TimeSlotReportView> TimeSlotReports { get; set; } = new();
}

public class TimeSlotReportView
{
    public string TimeSlotId { get; set; }
    public List<AttendanceStatusCount> AttendanceStatusCounts { get; set; } = new();
}

public class AttendanceStatusCount
{

    public string AttendanceStatusId { get; set; }
    public int? Count { get; set; } = null;
}

public class UserGroupReportView : GroupReportView
{
    public List<UserReportView> UserReports { get; set; } = new();
}

public class UserReportView
{
    public UserSummaryResponse User { get; set; } = new();

    public List<DayReportView> DayReports { get; set; } = new();
}