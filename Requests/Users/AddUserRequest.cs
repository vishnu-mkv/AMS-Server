using AMS.Enums;
using System.ComponentModel.DataAnnotations;

namespace AMS.Requests
{
    public class AddUserRequest
    {

        public string? Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public DateTime? DOB { get; set; }

        [Required]
        public bool SignInAllowed { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }

        // role ids
        public string[]? RoleIds { get; set; }
        // picture is a file

        public IFormFile? PictureFile { get; set; }

        // schedule id
        public string? ScheduleId { get; set; }

        // group ids
        public string[]? GroupIds { get; set; }
    }
}
