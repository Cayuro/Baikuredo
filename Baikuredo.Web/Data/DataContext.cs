using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Data.Entities;

namespace Baikuredo.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<BaikuredoRole> BaikuredoRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ConfigureKeys(builder);
            ConfigureIndexes(builder);

            base.OnModelCreating(builder);
        }

        private void ConfigureKeys(ModelBuilder builder)
        {
            // RolePermission (Composite primary key)
            builder.Entity<RolePermission>()
                .HasKey(rp => new { rp.BaikuredoRoleId, rp.PermissionId });

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.BaikuredoRole)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.BaikuredoRoleId);
        }

        private void ConfigureIndexes(ModelBuilder builder)
        {
            // Roles
            builder.Entity<BaikuredoRole>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // Categories
            builder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // Brands
            builder.Entity<Brand>()
                .HasIndex(b => b.Name)
                .IsUnique();

            // Users
            builder.Entity<User>()
                .HasIndex(u => u.Document)
                .IsUnique();

            // Permissions
            builder.Entity<Permission>()
                .HasIndex(p => p.Code)
                .IsUnique();
        }
    }
}
