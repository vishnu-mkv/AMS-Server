namespace AMS.Requests;

public class AddScheduleRequest
{

    public string Name { get; set; }
    public int[] Days { get; set; } = Array.Empty<int>();
    public string? Color { get; set; }
}