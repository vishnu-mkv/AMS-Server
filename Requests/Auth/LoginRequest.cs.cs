using System.ComponentModel.DataAnnotations;

namespace AMS.Requests
{
    public class LoginRequest
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}
