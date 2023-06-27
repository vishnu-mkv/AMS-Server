using AMS.Models;
using AMS.Interfaces;
using AMS.Providers;
using AMS.Requests;
using Microsoft.EntityFrameworkCore;

namespace AMS.Data.Seeders
{
    public class AdminUserSeeder
    {
        private readonly IServiceProvider _services;

        public AdminUserSeeder(IServiceProvider services)
        {
            _services = services;
        }

        public async void Seed()
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            // check if default organization exists
            var OrganizationProvider = scope.ServiceProvider.GetService<IOrganizationProvider>();

            var organization = OrganizationProvider.GetOrganizationById("default");

            // create default organization if it does not exist
            if (organization == null)
            {
                organization = OrganizationProvider.CreateOrganization("Default");
            }

            // check if admin access permission exists
            Permission AdminPermission = new(PermissionEnum.AdminAccess);

            var permission = context.Permissions.FirstOrDefault(permission => permission.Id == AdminPermission.Id) ?? throw new Exception("Admin Access permission does not exist");

            // check if admin role exists
            Role AdminRole = new("Administrator");

            // get IRolesProvider
            var RolesProvider = scope.ServiceProvider.GetService<IRoleProvider>();

            Role role = RolesProvider.GetRolesByIds(new string[] { AdminRole.Id }).FirstOrDefault();

            if (role == null || role.Organization.Id != organization.Id)
            {
                role = await RolesProvider.CreateRole("Administrator", "Administrator role", new List<Permission> { AdminPermission }, organization);
            }

            // check if admin user exists

            // get IUsersProvider
            var userManager = scope.ServiceProvider.GetService<IUserManager>();

            var user = context.Users.Include(u => u.Organization).FirstOrDefault(x => x.Roles.Any(y => y.Id == role.Id));

            if (user != null && user.Organization.Id == organization.Id) return;

            Console.WriteLine("Please provide the following information:");

            Console.Write("First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            AddUserRequest newUser = new()
            {
                FirstName = firstName,
                LastName = lastName,
                RoleIds = new[] { role.Id },
                Id = "1",
                Password = password,
                SignInAllowed = true,
                UserName = username
            };

            userManager.Register(newUser, organization);

            Console.WriteLine("Admin user created successfully!");
        }
    }
}
