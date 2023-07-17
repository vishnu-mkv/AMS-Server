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

            var OrganizationName = "KEC";
            var OrganizationId = OrganizationName.ToLower();

            // check if default organization exists
            var OrganizationProvider = scope.ServiceProvider.GetService<IOrganizationProvider>();

            Console.WriteLine("Organizations found: " + context.Organizations.Count());
            var organization = context.Organizations.FirstOrDefault(organization => organization.Name == OrganizationName);
            // create default organization if it does not exist
            if (organization == null)
            {
                organization = new Organization()
                {
                    Id = OrganizationId,
                    Name = OrganizationName,
                };
                context.Organizations.Add(organization);
                context.SaveChanges();
            }

            // check if admin access permission exists
            Permission AdminPermission = new(PermissionEnum.AdminAccess);

            var permission = context.Permissions.FirstOrDefault(permission => permission.Id == AdminPermission.Id) ?? throw new Exception("Admin Access permission does not exist");

            var roleId = OrganizationId + "-" + "administrator";
            // check if admin role exists
            var role = context.Roles.Include(x => x.Permissions).Where(x => x.OrganizationId == OrganizationId).FirstOrDefault(role => role.Id == roleId);

            if (role == null)
            {
                role = new Role()
                {
                    Id = roleId,
                    Name = "Administrator",
                    Permissions = new List<Permission>()
                    {
                        permission
                    },
                    Description = "Administrator role",
                    Organization = organization
                };
                context.Roles.Add(role);
                context.SaveChanges();
            }

            Console.WriteLine("Admin role found: " + role.Name);

            // get IUsersProvider
            var userManager = scope.ServiceProvider.GetService<IUserManager>();

            var user = context.Users.Include(u => u.Organization).Where(x => x.OrganizationId == OrganizationId).FirstOrDefault(x => x.Roles.Any(y => y.Id == role.Id));

            if (user != null && user.Organization.Id == organization.Id)
            {
                Console.WriteLine("User Found: " + user.FirstName);
                return;
            }

            Console.WriteLine("Please provide the following information:");

            Console.Write("First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            var passwordHasher = scope.ServiceProvider.GetService<ISecurePasswordHasher>();

            var NewUser = new ApplicationUser()
            {
                FirstName = firstName,
                LastName = lastName,
                Password = passwordHasher.Hash(password),
                Roles = new List<Role>{
                    role
                },
                OrganizationId = organization.Id,
                SignInAllowed = true,
                UserName = username,
            };

            context.Users.Add(NewUser);
            context.SaveChanges();

            Console.WriteLine("Admin user created successfully!");
        }
    }
}
