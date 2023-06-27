namespace AMS.Models;

public class Schedule
{
    public string Id { get; set; }
    public string Name { get; set; }

    // list of users
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    // organization
    public string OrganizationId { get; set; }

    public virtual Organization Organization { get; set; }

    public Schedule()
    {
        Id = Guid.NewGuid().ToString();
    }
}