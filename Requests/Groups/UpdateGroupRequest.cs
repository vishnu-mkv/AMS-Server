namespace AMS.Requests;

public class UpdateGroupRequest
{
    public string? Id { get; set; }
    public string? Name { get; set; } = null!;
    public string? Color { get; set; }
    public string[]? Users { get; set; }
    public string[]? Groups { get; set; }

    public bool? Disabled { get; set; } = false;
    public string? ScheduleId { get; set; }
}