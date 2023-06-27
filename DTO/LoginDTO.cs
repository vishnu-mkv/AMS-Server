using AMS.Models;

namespace AMS.DTO
{
    public class LoginDTO
    {
         public string Token { get; set; }
        public ApplicationUser User { get; set; }
    }
}
