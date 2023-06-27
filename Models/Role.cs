using System.ComponentModel.DataAnnotations;

namespace AMS.Models
{
    public class Role
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        // navigation properties
        public virtual ICollection<Permission> Permissions { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        [Required]
        public Organization? Organization { get; set; } = null;

        public string? OrganizationId { get; set; }

        public string? Color { get; set; } = "#000000";

        public Role()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Role(string name)
        {
            Name = name;
            Description = "";
            Id = name.ToLower().Replace(" ", "-");
        }

        public Role(string name, string description, ICollection<Permission> permissions)
        {
            Name = name;
            Description = description;
            Id = name.ToLower().Replace(" ", "-");
            Permissions = permissions;
        }

        // override equality
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var _role = (Role)obj;
            return _role.Id == Id;
        }
    }
}
