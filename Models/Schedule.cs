using System.ComponentModel.DataAnnotations.Schema;

namespace AMS.Models;

public class Schedule
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; } = "#ccc";

    public IEnumerable<int> Days { get; set; } = Array.Empty<int>();
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public ApplicationUser? CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public string? CreatedById { get; set; }

    // list of users who follow this schedule
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();


    // organization
    public string OrganizationId { get; set; }

    public virtual Organization Organization { get; set; }

    public Schedule()
    {
        Id = Guid.NewGuid().ToString();
    }
}