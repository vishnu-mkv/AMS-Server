namespace AMS.Responses;

public class RecordResponse
{
    public string Id { get; set; }

    public UserSummaryResponse? User { get; set; }

    public string? AttendanceStatusId { get; set; }
}