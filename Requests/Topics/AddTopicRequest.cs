using System.ComponentModel.DataAnnotations;

namespace AMS.Requests;

public class AddTopicRequest
{
    [Required]
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
}