using System.ComponentModel.DataAnnotations;
using AMS.Models;

namespace AMS.Requests;

public class AddGroupRequest
{
    public string? Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public GroupType GroupType { get; set; }
    public string[]? Users { get; set; }
    public string[]? Groups { get; set; }

    public string? ScheduleId { get; set; } = null!;
}