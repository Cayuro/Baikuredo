using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Data.Entities;

namespace Baikuredo.Web.Data.Seeders
{
    public class BrandsSeeder
    {
        private readonly DataContext _context;

        public BrandsSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            List<string> names = new List<string> { "Yamaha", "Suzuki", "Bajaj", "Honda", "KTM", "TVS", "AKT", "Kawasaki" };

            foreach (string name in names)
            {
                bool exists = await _context.Brands.AnyAsync(b => b.Name == name);

                if (!exists)
                {
                    await _context.Brands.AddAsync(new Brand { Id = Guid.NewGuid(), Name = name, IsActive = true });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
