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

            modelBuilder.Entity<Group>().HasMany(g => g.Users).WithMany(h => h.Groups).UsingEntity(
                "GroupUser",
                l => l.HasOne(typeof(ApplicationUser)).WithMany().HasForeignKey("UsersId").HasPrincipalKey(nameof(ApplicationUser.Id)),
                r => r.HasOne(typeof(Group)).WithMany().HasForeignKey("GroupsId").HasPrincipalKey(nameof(Group.Id)),
                j => j.HasKey("UsersId", "GroupsId")
            );

            // group createdBy
            modelBuilder.Entity<Group>().HasOne(g => g.CreatedBy).WithMany().HasForeignKey(g => g.CreatedById).OnDelete(DeleteBehavior.SetNull);
        }


        public DbSet<Organization> Organizations { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Group> Groups { get; set; }

        public DbSet<Topic> Topics { get; set; }
    }
}
