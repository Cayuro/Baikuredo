using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Core;
using Baikuredo.Web.Data.Entities;

namespace Baikuredo.Web.Data.Seeders
{
    public class AccountSeeder
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        private const string ASSISTANT_ROLE_NAME = "Auxiliar";
        private const string CLIENT_ROLE_NAME = "Cliente";
        private const string DEFAULT_PASSWORD = "1234";

        public AccountSeeder(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            await CheckRolesAsync();
            await CheckUsersAsync();
        }

        // ROLES

        private async Task CheckRolesAsync()
        {
            // Admin: no necesita permisos, el AccountService lo deja pasar a todo
            await CheckRoleAsync(Env.SUPER_ADMIN_ROLE_NAME, new List<string>());

            // Auxiliar: gestiona el catálogo, las motos, las publicaciones y las reservas
            await CheckRoleAsync(ASSISTANT_ROLE_NAME, new List<string> { "Marcas", "Modelos", "Categorias", "Propietarios", "Motos", "Publicaciones", "Reservas" });

            // Cliente: por ahora sin permisos de administración
            await CheckRoleAsync(CLIENT_ROLE_NAME, new List<string>());
        }

        private async Task CheckRoleAsync(string roleName, List<string> modules)
        {
            BaikuredoRole? role = await _context.BaikuredoRoles.FirstOrDefaultAsync(r => r.Name == roleName);

            if (role is null)
            {
                role = new BaikuredoRole
                {
                    Id = Guid.NewGuid(),
                    Name = roleName
                };

                await _context.BaikuredoRoles.AddAsync(role);
            }

            Guid roleId = role.Id;

            List<Permission> permissions = await _context.Permissions.Where(p => modules.Contains(p.Module))
                                                                     .ToListAsync();

            foreach (Permission permission in permissions)
            {
                // Si el rol ya existía, solo se agregan los permisos que le falten
                bool hasPermission = await _context.RolePermissions.AnyAsync(rp => rp.BaikuredoRoleId == roleId && rp.PermissionId == permission.Id);

                if (!hasPermission)
                {
                    await _context.RolePermissions.AddAsync(new RolePermission { BaikuredoRoleId = roleId, PermissionId = permission.Id });
                }
            }

            await _context.SaveChangesAsync();
        }

        // USERS

        private async Task CheckUsersAsync()
        {
            await CheckUserAsync("admin@baikuredo.com", "Administrador", "Baikuredo", "1111", "3000000001", Env.SUPER_ADMIN_ROLE_NAME);
            await CheckUserAsync("auxiliar@baikuredo.com", "Auxiliar", "Baikuredo", "2222", "3000000002", ASSISTANT_ROLE_NAME);
            await CheckUserAsync("cliente@baikuredo.com", "Cliente", "Baikuredo", "3333", "3000000003", CLIENT_ROLE_NAME);
        }

        private async Task CheckUserAsync(string email, string firstName, string lastName, string document, string phone, string roleName)
        {
            User? user = await _userManager.FindByEmailAsync(email);

            if (user is not null)
            {
                return;
            }

            BaikuredoRole role = await _context.BaikuredoRoles.FirstAsync(r => r.Name == roleName);

            user = new User
            {
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName,
                Document = document,
                PhoneNumber = phone,
                BaikuredoRoleId = role.Id,
                EmailConfirmed = true
            };

            IdentityResult result = await _userManager.CreateAsync(user, DEFAULT_PASSWORD);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"No se pudo crear el usuario {email}: {errors}");
            }
        }
    }
}
