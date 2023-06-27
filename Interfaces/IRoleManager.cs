using AMS.Models;
using AMS.Requests;

namespace AMS.Interfaces;

public interface IRoleManager
{

    // add role from request
    Role AddRole(AddRoleRequest request);

    Role UpdateRole(UpdateRoleRequest request, string id);

    public List<Role> GetRoles();

    public Role GetRoleById(string id);

    public void DeleteRole(string id);

}