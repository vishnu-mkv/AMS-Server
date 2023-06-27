using System.ComponentModel.DataAnnotations;
using AMS.Providers;

namespace AMS.Models
{
    public class Permission
    {
        // id is slugified name
        [Key]
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        // navigation properties
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

        public Permission() { }

        public Permission(string name, string description)
        {
            Name = name;
            Description = description;
            Id = name.ToLower().Replace(" ", "-");
        }

        public Permission(PermissionEnum permission)
        {
            var _p = PermissionRepo.PermissionMap.GetValueOrDefault(permission);
            Name = _p.Name;
            Description = _p.Description;
            Id = Name.ToLower().Replace(" ", "-");
        }

        // override equality
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var _permission = (Permission)obj;
            return _permission.Id == Id;
        }
    }
}
