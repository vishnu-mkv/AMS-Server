namespace AMS.Requests;

public class UpdateRoleRequest
{
    public string? Name { get; set; } = null!;
    public string[]? Permissions { get; set; }

    public string[]? Users { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; } = null!;
}