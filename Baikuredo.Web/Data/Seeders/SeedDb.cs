using Microsoft.AspNetCore.Identity;
using Baikuredo.Web.Data.Entities;

namespace Baikuredo.Web.Data.Seeders
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public SeedDb(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            await new CategoriesSeeder(_context).SeedAsync();
            await new BrandsSeeder(_context).SeedAsync();
            await new PermissionsSeeder(_context).SeedAsync();
            await new AccountSeeder(_context, _userManager).SeedAsync();
        }
    }
}
