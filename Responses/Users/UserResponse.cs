using AMS.Models;

namespace AMS.Responses
{
    public class UserResponse : ApplicationUser
    {
        //roles
        new public RoleResponse[] Roles { get; set; } = Array.Empty<RoleResponse>();

        new public GroupSummaryResponse[] Groups { get; set; } = Array.Empty<GroupSummaryResponse>();
    }
}
