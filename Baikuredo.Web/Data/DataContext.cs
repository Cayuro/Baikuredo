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
        public DbSet<MotorcycleModel> MotorcycleModels { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Motorcycle> Motorcycles { get; set; }
        public DbSet<MotorcyclePublication> MotorcyclePublications { get; set; }
        public DbSet<MotorcyclePhoto> MotorcyclePhotos { get; set; }
        public DbSet<PriceHistory> PriceHistories { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<BaikuredoRole> BaikuredoRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ConfigureKeys(builder);
            ConfigureBusinessRelations(builder);
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

            // MotorcycleModel -> Brand (no permite borrar una marca que tenga modelos)
            builder.Entity<MotorcycleModel>()
                .HasOne(m => m.Brand)
                .WithMany()
                .HasForeignKey(m => m.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureBusinessRelations(ModelBuilder builder)
        {
            // Regla general: Restrict = no se puede borrar un registro si otros dependen de él.
            // Solo las fotos, el historial y las reservas se borran junto con su publicación (Cascade).

            // Motorcycle -> MotorcycleModel, Category, Owner
            builder.Entity<Motorcycle>()
                .HasOne(m => m.MotorcycleModel)
                .WithMany()
                .HasForeignKey(m => m.MotorcycleModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Motorcycle>()
                .HasOne(m => m.Category)
                .WithMany()
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Motorcycle>()
                .HasOne(m => m.Owner)
                .WithMany(o => o.Motorcycles)
                .HasForeignKey(m => m.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // MotorcyclePublication -> Motorcycle, User
            builder.Entity<MotorcyclePublication>()
                .HasOne(p => p.Motorcycle)
                .WithMany(m => m.Publications)
                .HasForeignKey(p => p.MotorcycleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MotorcyclePublication>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // El estado se guarda como texto (Available, Sold...) y no como número
            builder.Entity<MotorcyclePublication>()
                .Property(p => p.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // MotorcyclePhoto -> MotorcyclePublication
            builder.Entity<MotorcyclePhoto>()
                .HasOne(f => f.MotorcyclePublication)
                .WithMany(p => p.Photos)
                .HasForeignKey(f => f.MotorcyclePublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // PriceHistory -> MotorcyclePublication, User
            builder.Entity<PriceHistory>()
                .HasOne(h => h.MotorcyclePublication)
                .WithMany(p => p.PriceHistories)
                .HasForeignKey(h => h.MotorcyclePublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PriceHistory>()
                .HasOne(h => h.User)
                .WithMany()
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reservation -> MotorcyclePublication, User
            builder.Entity<Reservation>()
                .HasOne(r => r.MotorcyclePublication)
                .WithMany(p => p.Reservations)
                .HasForeignKey(r => r.MotorcyclePublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reservation>()
                .Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // AuditLog -> User
            builder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureIndexes(ModelBuilder builder)
        {
            // Owners (el mismo documento no se repite para el mismo tipo)
            builder.Entity<Owner>()
                .HasIndex(o => new { o.DocumentType, o.Document })
                .IsUnique();

            // Motorcycles (la placa es única)
            builder.Entity<Motorcycle>()
                .HasIndex(m => m.Plate)
                .IsUnique();

            // MotorcyclePublications (se filtra mucho por estado)
            builder.Entity<MotorcyclePublication>()
                .HasIndex(p => p.Status);

            // AuditLogs (se consulta por fecha)
            builder.Entity<AuditLog>()
                .HasIndex(a => a.CreatedAt);

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

            // MotorcycleModels (el mismo nombre no se repite dentro de una marca)
            builder.Entity<MotorcycleModel>()
                .HasIndex(m => new { m.BrandId, m.Name })
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
