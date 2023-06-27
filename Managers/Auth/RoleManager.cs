using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using AMS.Requests;
using Microsoft.EntityFrameworkCore;

namespace AMS.Managers;

public class RoleManager : IRoleManager
{
    private readonly IRoleProvider _roleProvider;
    private readonly IAuthManager _authManager;
    private readonly ApplicationDbContext context;

    public RoleManager(IRoleProvider roleProvider, IAuthManager authManager, ApplicationDbContext context)
    {
        _roleProvider = roleProvider;
        _authManager = authManager;
        this.context = context;
    }

    public Role AddRole(AddRoleRequest request)
    {
        // get permissions from request

        List<Permission> permissions = request.Permissions != null ? context.Permissions.Where(p => request.Permissions.Contains(p.Id)).ToList() : new List<Permission>();

        Role role = new()
        {
            Name = request.Name,
            Description = request.Description,
            Color = request.Color,
            // unique permissions
            Permissions = permissions,
            OrganizationId = _authManager.GetUserOrganization().Id
        };

        if (request.Id != null)
        {
            role.Id = request.Id;
        }


        role.Users = request.Users == null ? new List<ApplicationUser>() : context.Users.Where(p => request.Users.Contains(p.Id)).ToList();

        context.Roles.Add(role);
        context.SaveChanges();

        // add role to role provider
        _roleProvider.AddRole(role);

        return role;
    }


    // update role
    public Role UpdateRole(UpdateRoleRequest request, string id)
    {

        Role? role = context.Roles.Include(p => p.Permissions).Include(p => p.Users).FirstOrDefault(r => r.OrganizationId == _authManager.GetUserOrganization().Id && r.Id == id) ?? throw new Exception("Role with id " + id + " does not exist.");

        // update role
        if (request.Name != null)
        {
            role.Name = request.Name;
        }

        if (request.Description != null)
        {
            role.Description = request.Description;
        }

        if (request.Color != null)
        {
            role.Color = request.Color;
        }

        // remove permissions that are not in request and add permissions that are in request
        if (request.Permissions != null)
        {
            List<Permission> permissions = context.Permissions.Where(p => request.Permissions.Contains(p.Id)).ToList();
            role.Permissions = permissions;
        }

        // remove users that are not in request and add users that are in request
        if (request.Users != null)
        {
            List<ApplicationUser> users = context.Users.Where(p => request.Users.Contains(p.Id)).ToList();
            role.Users = users;
        }

        context.Roles.Update(role);
        context.SaveChanges();

        _roleProvider.AddRole(role);

        return role;
    }

    // get roles by organization id
    // get organization id from user manager
    public List<Role> GetRoles()
    {
        string organizationId = _authManager.GetUserOrganization().Id;

        List<Role> roles = context.Roles.Where(p => p.OrganizationId == organizationId).ToList();

        return roles;
    }

    public Role GetRoleById(string id)
    {
        Role role = context.Roles.Include(p => p.Permissions).Include(p => p.Users).FirstOrDefault(r => r.OrganizationId == _authManager.GetUserOrganization().Id && r.Id == id) ?? throw new Exception("Role with id " + id + " does not exist.");
        return role;
    }

    public void DeleteRole(string id)
    {
        Role role = context.Roles.FirstOrDefault(r => r.OrganizationId == _authManager.GetUserOrganization().Id && r.Id == id) ?? throw new Exception("Role with id " + id + " does not exist.");

        context.Roles.Remove(role);
        context.SaveChanges();

        _roleProvider.RemoveRole(role.Id);
    }
}