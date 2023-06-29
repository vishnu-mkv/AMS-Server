using AMS.DTO;
using AMS.Models;
using AMS.Requests;

namespace AMS.Interfaces
{
    public interface IUserManager
    {
        public ApplicationUser? GetUserById(string id, bool includeRoles = false, bool includeSchedule = false, bool includeGroups = false);

        public bool IsUniqueID(string id);

        public bool IsUniqueUserName(string username);

        ApplicationUser UpdateUser(UpdateUserRequest request, string id);

        ApplicationUser Register(AddUserRequest request);

        ApplicationUser Register(AddUserRequest request, Organization organization);

        ApplicationUser GetUserByUsername(string username);
        public bool UserExists(string organizationId, string id);
        public PaginationDTO<ApplicationUser> ListUsers(UserPaginationQuery paginationQuery);

        public bool CheckUsersHaveSchedule(string? scheduleId, string[] users);
    }
}
