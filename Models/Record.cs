using AMS.Models;


public class Record
{

    public string Id { get; set; }

    public Attendance Attendance { get; set; }

    public string AttendanceId { get; set; }

    public ApplicationUser? User { get; set; }

    public string? UserId { get; set; }

    public AttendanceStatus? Status { get; set; }

    public string? AttendanceStatusId { get; set; }


    public Record()
    {
        Id = Guid.NewGuid().ToString();
    }
}