
using AMS.Models;
using AMS.Providers;

namespace AMS.Data.Seeders
{
    public class PermissionSeeder
    {
        private readonly IServiceProvider _services;

        public PermissionSeeder(IServiceProvider services)
        {
            _services = services;
        }

        public void Seed()
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            foreach (PermissionEnum permission in Enum.GetValues(typeof(PermissionEnum)))
            {
                var permissionData = new Permission(permission);
                if (!context.Permissions.Any(p => p.Id == permissionData.Id))
                {
                    context.Permissions.Add(permissionData);
                }
            }

            context.SaveChanges();
        }
    }
}
