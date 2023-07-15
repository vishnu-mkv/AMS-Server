using System.ComponentModel.DataAnnotations;

namespace AMS.Requests;

public class AttendanceReportRequest
{
    public DateTime? StartDate { get; set; } = null;

    public DateTime? EndDate { get; set; } = null;

    [Required]
    public string GroupId { get; set; } = null!;

    public string[]? TopicIds { get; set; } = null;

    public string[]? TimeSlotIds { get; set; } = null;

    public DayOfWeek[]? Days { get; set; } = null;
    public string[]? AttendanceStatusIds { get; set; } = null;

}