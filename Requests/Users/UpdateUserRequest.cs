using AMS.Enums;

namespace AMS.Requests
{
    public class UpdateUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public bool? SignInAllowed { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string[]? RoleIds { get; set; }
        public IFormFile? PictureFile { get; set; }

        public bool? Disabled { get; set; }
        public string? ScheduleId { get; set; }
        public string[]? GroupIds { get; set; }
    }

}
