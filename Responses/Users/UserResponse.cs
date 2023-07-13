using System.Text.Json.Serialization;
using AMS.Enums;
using AMS.Models;
using AMS.Utils;

namespace AMS.Responses
{
    public class UserResponse
    {
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

        [JsonConverter(typeof(PhotoUrlConverter))]

        public string Picture { get; set; } = string.Empty;

        // disabled
        public bool Disabled { get; set; } = false;
        public Organization? Organization { get; set; }
        //roles
        public RoleResponse[] Roles { get; set; } = Array.Empty<RoleResponse>();

        public GroupSummaryResponse[] Groups { get; set; } = Array.Empty<GroupSummaryResponse>();

        public string? ScheduleId { get; set; }

        public ScheduleResponse? Schedule { get; set; }

        public string? UserName { get; set; }
    }
}
