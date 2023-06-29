using AMS.Enums;
using AMS.Utils;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AMS.Models
{
    public class ApplicationUser
    {

        [Key]
        public string Id { get; set; }

        // first name 
        public string FirstName { get; set; } = string.Empty;

        // last name
        public string LastName { get; set; } = string.Empty;

        // date of birth
        public DateTime? DOB { get; set; }

        // gender
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Gender Gender { get; set; }

        // sign in allowed
        public bool SignInAllowed { get; set; } = false;

        // username
        public string? UserName { get; set; } = string.Empty;

        // password
        [JsonIgnore]
        public string? Password { get; set; } = string.Empty;

        // picture
        [JsonConverter(typeof(PhotoUrlConverter))]
        public string Picture { get; set; } = string.Empty;

        // disabled
        public bool Disabled { get; set; } = false;

        // navigation properties
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

        public Organization? Organization { get; set; } = null!;

        [JsonIgnore]
        public string? OrganizationId { get; set; } = null!;

        // created at
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // schedule
        [JsonIgnore]
        public virtual Schedule? Schedule { get; set; } = null;

        public string? ScheduleId { get; set; } = null;

        // a user can be in many groups
        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

}

