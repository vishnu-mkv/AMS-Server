using AMS.DTO;
using AMS.Models;

namespace AMS.Interfaces;
public interface IAuthManager
{
    public ApplicationUser? GetCurrentUser();
    public string GetCurrentUserId();
    public string GetUserOrganizationId();
    public Organization GetUserOrganization();
    public LoginDTO Login(string username, string password);

}