namespace AMS.Requests;

public class UpdateScheduleRequest
{

    public string? Name { get; set; }
    public int[]? Days { get; set; }

    public string? Color { get; set; }
}