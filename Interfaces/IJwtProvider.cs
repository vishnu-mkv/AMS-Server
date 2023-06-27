using AMS.Models;

namespace AMS.Interfaces
{
    public interface IJwtProvider
    {
        public string GenerateToken(ApplicationUser user);
    }
}
