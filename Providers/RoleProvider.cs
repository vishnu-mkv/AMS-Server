using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Providers;

public class RoleProvider : IRoleProvider
{
    private Dictionary<string, Role> _roles = new();
    public Dictionary<string, Permission> Permissions { get; set; } = new();
    private readonly IServiceScopeFactory _serviceScope;

    // constructor with IServiceScopeFactory
    public RoleProvider(IServiceScopeFactory serviceScope)
    {
        _serviceScope = serviceScope;
        Initialize();
    }

    async Task Initialize()
    {
        await ExecuteWithContext(async context =>
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            // load all roles and permissions
            var roles = await context.Roles.AsNoTracking().Include(r => r.Permissions).Include(r => r.Organization).AsNoTracking().ToListAsync();
            _roles = roles.ToDictionary(r => r.Id);

            var permissions = await context.Permissions.AsNoTracking().ToListAsync();
            Permissions = permissions.ToDictionary(p => p.Id);
            return true;
        });
    }

    public IEnumerable<Permission> GetPermissions(string[] roleIds)
    {
        var userRoles = roleIds.Select(roleId => _roles.GetValueOrDefault(roleId));
        var permissions = userRoles.SelectMany(r => r?.Permissions ?? Enumerable.Empty<Permission>());
        return permissions;
    }

    public bool CheckForPermission(string[] roleIds, Permission permission)
    {
        IEnumerable<Permission> permissions = GetPermissions(roleIds);

        return permissions.Contains(permission);
    }

    public List<Role> GetRolesByIds(string[] roleIds)
    {
        var roles = roleIds.Select(roleId => _roles.GetValueOrDefault(roleId)).Where(r => r != null).ToList();
        return roles;
    }

    public bool RoleExists(string roleId)
    {
        return _roles.ContainsKey(roleId);
    }

    public bool PermissionExists(Permission permission)
    {
        return Permissions.ContainsValue(permission);
    }

    private async Task<T> ExecuteWithContext<T>(Func<ApplicationDbContext, Task<T>> action)
    {
        if (_serviceScope == null)
            throw new Exception("Service scope is not initialized.");

        using var scope = _serviceScope.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        return await action(context);
    }

    public bool CheckRolesExists(string[] roleIds)
    {
        return roleIds.All(RoleExists);
    }

    public List<Permission?> GetPermissionsById(PermissionEnum[] permissionEnums)
    {
        // convert enum to permission 
        return permissionEnums.Select(permissionEnum => Permissions.GetValueOrDefault(new Permission(permissionEnum).Id)).ToList();
    }

    public Task<Role> CreateRole(string name, string description, List<Permission> permissions, Organization organization)
    {
        return ExecuteWithContext(async context =>
        {
            context.Permissions.AttachRange(permissions);
            context.Organizations.Attach(organization);

            var role = new Role(name)
            {
                Description = description,
                Organization = organization,
                Permissions = permissions
            };

            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
            _roles.Add(role.Id, role);
            return role;
        });
    }

    public void AddRole(Role role)
    {
        // if exists update
        if (_roles.ContainsKey(role.Id))
        {
            _roles[role.Id] = role;
        }
        else
        {
            _roles.Add(role.Id, role);
        }
    }

    public void RemoveRole(string id)
    {
        if (_roles.ContainsKey(id))
        {
            _roles.Remove(id);
        }
    }
}
