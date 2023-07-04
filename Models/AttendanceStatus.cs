using System.Text.Json.Serialization;

namespace AMS.Models;

public class AttendanceStatus
{
    public string Id { get; set; }
    public string Name { get; set; }

    public string ShortName { get; set; }

    public string Color { get; set; } = "#000000";

    public string? OrganizationId { get; set; }

    [JsonIgnore]
    public Organization? Organization { get; set; }

    public AttendanceStatus()
    {
        Id = Guid.NewGuid().ToString();
    }

}

