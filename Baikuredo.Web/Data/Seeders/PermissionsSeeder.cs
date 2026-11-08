using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Data.Entities;

namespace Baikuredo.Web.Data.Seeders
{
    public class PermissionsSeeder
    {
        private readonly DataContext _context;

        public PermissionsSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            List<Permission> permissions = [..Brands(), ..MotorcycleModels(), ..Categories(), ..Owners(), ..Motorcycles(), ..Publications(), ..Reservations(), ..AuditLogs(), ..Users(), ..Roles()];

            foreach (Permission permission in permissions)
            {
                bool exists = await _context.Permissions.AnyAsync(p => p.Code == permission.Code);

                if (!exists)
                {
                    await _context.Permissions.AddAsync(permission);
                }
            }

            await _context.SaveChangesAsync();
        }

        private List<Permission> Brands()
        {
            return new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Code = "brands.show", Description = "Ver Marcas", Module = "Marcas" },
                new Permission { Id = Guid.NewGuid(), Code = "brands.create", Description = "Crear Marcas", Module = "Marcas" },
                new Permission { Id = Guid.NewGuid(), Code = "brands.update", Description = "Editar Marcas", Module = "Marcas" },
                new Permission { Id = Guid.NewGuid(), Code = "brands.delete", Description = "Eliminar Marcas", Module = "Marcas" },
            };
        }

        private List<Permission> MotorcycleModels()
        {
            return new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Code = "models.show", Description = "Ver Modelos", Module = "Modelos" },
                new Permission { Id = Guid.NewGuid(), Code = "models.create", Description = "Crear Modelos", Module = "Modelos" },
                new Permission { Id = Guid.NewGuid(), Code = "models.update", Description = "Editar Modelos", Module = "Modelos" },
                new Permission { Id = Guid.NewGuid(), Code = "models.delete", Description = "Eliminar Modelos", Module = "Modelos" },
            };
        }

        private List<Permission> Categories()
        {
            return new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Code = "categories.show", Description = "Ver Categorías", Module = "Categorias" },
                new Permission { Id = Guid.NewGuid(), Code = "categories.create", Description = "Crear Categorías", Module = "Categorias" },
                new Permission { Id = Guid.NewGuid(), Code = "categories.update", Description = "Editar Categorías", Module = "Categorias" },
                new Permission { Id = Guid.NewGuid(), Code = "categories.delete", Description = "Eliminar Categorías", Module = "Categorias" },
            };
        }

        private List<Permission> Owners()
        {
            return new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Code = "owners.show", Description = "Ver Propietarios", Module = "Propietarios" },
                new Permission { Id = Guid.NewGuid(), Code = "owners.create", Description = "Crear Propietarios", Module = "Propietarios" },
                new Permission { Id = Guid.NewGuid(), Code = "owners.update", Description = "Editar Propietarios", Module = "Propietarios" },
                new Permission { Id = Guid.NewGuid(), Code = "owners.delete", Description = "Eliminar Propietarios", Module = "Propietarios" },
            };
        }

        private List<Permission> Motorcycles()
        {
            return new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Code = "motorcycles.show", Description = "Ver Motos", Module = "Motos" },
                new Permission { Id = Guid.NewGuid(), Code = "motorcycles.create", Description = "Crear Motos", Module = "Motos" },
                new Permission { Id = Guid.NewGuid(), Code = "motorcycles.update", Description = "Editar Motos", Module = "Motos" },
                new Permission { Id = Guid.NewGuid(), Code = "motorcycles.delete", Description = "Eliminar Motos", Module = "Motos" },
            };
        }

        private List<Permission> Publications()
        {
            return new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Code = "publications.show", Description = "Ver Publicaciones", Module = "Publicaciones" },
                new Permission { Id = Guid.NewGuid(), Code = "publications.create", Description = "Crear Publicaciones", Module = "Publicaciones" },
                new Permission { Id = Guid.NewGuid(), Code = "publications.update", Description = "Editar Publicaciones", Module = "Publicaciones" },
                new Permission { Id = Guid.NewGuid(), Code = "publications.delete", Description = "Eliminar Publicaciones", Module = "Publicaciones" },
            };
        }

        private List<Permission> Reservations()
        {
            return new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Code = "reservations.show", Description = "Ver Reservas", Module = "Reservas" },
                new Permission { Id = Guid.NewGuid(), Code = "reservations.create", Description = "Crear Reservas", Module = "Reservas" },
                new Permission { Id = Guid.NewGuid(), Code = "reservations.update", Description = "Editar Reservas", Module = "Reservas" },
                new Permission { Id = Guid.NewGuid(), Code = "reservations.delete", Description = "Eliminar Reservas", Module = "Reservas" },
            };
        }

        private List<Permission> AuditLogs()
        {
            return new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Code = "auditlogs.show", Description = "Ver Auditoría", Module = "Auditoria" },
            };
        }

        private List<Permission> Users()
        {
            return new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Code = "users.show", Description = "Ver Usuarios", Module = "Usuarios" },
                new Permission { Id = Guid.NewGuid(), Code = "users.create", Description = "Crear Usuarios", Module = "Usuarios" },
                new Permission { Id = Guid.NewGuid(), Code = "users.update", Description = "Editar Usuarios", Module = "Usuarios" },
                new Permission { Id = Guid.NewGuid(), Code = "users.delete", Description = "Eliminar Usuarios", Module = "Usuarios" },
            };
        }

        private List<Permission> Roles()
        {
            return new List<Permission>
            {
                new Permission { Id = Guid.NewGuid(), Code = "roles.show", Description = "Ver Roles", Module = "Roles" },
                new Permission { Id = Guid.NewGuid(), Code = "roles.create", Description = "Crear Roles", Module = "Roles" },
                new Permission { Id = Guid.NewGuid(), Code = "roles.update", Description = "Editar Roles", Module = "Roles" },
                new Permission { Id = Guid.NewGuid(), Code = "roles.delete", Description = "Eliminar Roles", Module = "Roles" },
            };
        }
    }
}
