namespace AMS.Requests;

public class UpdateGroupRequest
{
    public string? Id { get; set; }
    public string? Name { get; set; } = null!;
    public string? Color { get; set; }
    public string[]? Users_to_add { get; set; }
    public string[]? Users_to_remove { get; set; }
    public string[]? Groups_to_add { get; set; }
    public string[]? Groups_to_remove { get; set; }

    public bool? Disabled { get; set; } = false;
}