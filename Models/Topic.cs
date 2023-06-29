using System.Text.Json.Serialization;

namespace AMS.Models;

public class Topic
{

    public string Id { get; set; }

    public string Name { get; set; }

    public string Color { get; set; } = "#ccc";

    public string? OrganizationId { get; set; }

    [JsonIgnore]
    public Organization? Organization { get; set; }

    public Topic()
    {
        Id = Guid.NewGuid().ToString();
    }
}