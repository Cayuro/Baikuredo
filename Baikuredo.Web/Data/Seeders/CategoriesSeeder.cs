using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Data.Entities;

namespace Baikuredo.Web.Data.Seeders
{
    public class CategoriesSeeder
    {
        private readonly DataContext _context;

        public CategoriesSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            List<Category> categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Scooter", Description = "Motos automáticas urbanas de bajo cilindraje" },
                new Category { Id = Guid.NewGuid(), Name = "Deportiva", Description = "Motos de alto rendimiento y diseño aerodinámico" },
                new Category { Id = Guid.NewGuid(), Name = "Naked", Description = "Motos sin carenado, estilo urbano-deportivo" },
                new Category { Id = Guid.NewGuid(), Name = "Enduro", Description = "Motos todoterreno para caminos sin pavimentar" },
                new Category { Id = Guid.NewGuid(), Name = "Touring", Description = "Motos de gran cilindrada para viajes largos" },
                new Category { Id = Guid.NewGuid(), Name = "Café Racer", Description = "Motos clásicas con estética retro y deportiva" },
            };

            foreach (Category category in categories)
            {
                bool exists = await _context.Categories.AnyAsync(c => c.Name == category.Name);

                if (!exists)
                {
                    await _context.Categories.AddAsync(category);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
