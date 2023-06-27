using AMS.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Schedule>().HasMany(s => s.Users).WithOne(u => u.Schedule).OnDelete(DeleteBehavior.SetNull);
        }


        public DbSet<Organization> Organizations { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
    }
}
