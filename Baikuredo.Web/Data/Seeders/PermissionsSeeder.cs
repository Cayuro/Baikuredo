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
            List<Permission> permissions = [..Brands(), ..Categories(), ..Users(), ..Roles()];

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
