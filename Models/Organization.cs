using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AMS.Models;

public class Organization
{

    [Key]
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";

    [JsonIgnore]
    // navigation properties
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    [JsonIgnore]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    [JsonIgnore]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public Organization()
    {
        Id = Guid.NewGuid().ToString();
    }

    public Organization(string name)
    {
        Id = name.ToLower().Replace(" ", "-").ToString();
        Name = name;
    }
}