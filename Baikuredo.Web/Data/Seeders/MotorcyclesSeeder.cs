using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Data.Entities;

namespace Baikuredo.Web.Data.Seeders
{
    public class MotorcyclesSeeder
    {
        private readonly DataContext _context;

        public MotorcyclesSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await CheckMotorcycleAsync("ABC12D", 2022, 890, "Azul", "Yamaha", "MT-09", "Naked", "1000000001");
            await CheckMotorcycleAsync("XYZ34E", 2021, 155, "Negro", "Yamaha", "NMAX 155", "Scooter", "1000000002");
            await CheckMotorcycleAsync("KTM56F", 2023, 390, "Naranja", "KTM", "Duke 390", "Naked", "900123456");

            await _context.SaveChangesAsync();
        }

        private async Task CheckMotorcycleAsync(string plate, int year, int displacement, string color, string brandName, string modelName, string categoryName, string ownerDocument)
        {
            bool exists = await _context.Motorcycles.AnyAsync(m => m.Plate == plate);

            if (exists)
            {
                return;
            }

            MotorcycleModel? model = await _context.MotorcycleModels.FirstOrDefaultAsync(m => m.Name == modelName && m.Brand!.Name == brandName);
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            Owner? owner = await _context.Owners.FirstOrDefaultAsync(o => o.Document == ownerDocument);

            // Si falta alguno de los datos relacionados, no se crea la moto de ejemplo
            if (model is null || category is null || owner is null)
            {
                return;
            }

            await _context.Motorcycles.AddAsync(new Motorcycle
            {
                Id = Guid.NewGuid(),
                Plate = plate,
                Year = year,
                Displacement = displacement,
                Color = color,
                MotorcycleModelId = model.Id,
                CategoryId = category.Id,
                OwnerId = owner.Id
            });
        }
    }
}
