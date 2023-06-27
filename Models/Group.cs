
using System.ComponentModel.DataAnnotations.Schema;

namespace AMS.Models;

public enum GroupType
{
    GroupOfUsers,
    GroupOfGroups,
}

public class Group
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; } = "#ccc";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ApplicationUser? CreatedBy { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public string? CreatedById { get; set; }
    public Organization? Organization { get; set; }
    public string? OrganizationId { get; set; }
    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public GroupType GroupType { get; set; }

    public Group()
    {
        Id = Guid.NewGuid().ToString();
    }
}