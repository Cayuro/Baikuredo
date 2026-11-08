using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Data.Entities;

namespace Baikuredo.Web.Data.Seeders
{
    public class MotorcycleModelsSeeder
    {
        private readonly DataContext _context;

        public MotorcycleModelsSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            Dictionary<string, List<string>> modelsByBrand = new Dictionary<string, List<string>>
            {
                { "Yamaha", new List<string> { "MT-09", "FZ-25", "NMAX 155" } },
                { "Suzuki", new List<string> { "Gixxer 250", "GSX-S150" } },
                { "Bajaj", new List<string> { "Pulsar NS200", "Dominar 400" } },
                { "Honda", new List<string> { "CB190R", "XR150L" } },
                { "KTM", new List<string> { "Duke 200", "Duke 390" } },
                { "TVS", new List<string> { "Apache 200" } },
                { "AKT", new List<string> { "NKD 125" } },
                { "Kawasaki", new List<string> { "Ninja 400", "Z400" } },
            };

            foreach (KeyValuePair<string, List<string>> item in modelsByBrand)
            {
                Brand? brand = await _context.Brands.FirstOrDefaultAsync(b => b.Name == item.Key);

                if (brand is null)
                {
                    continue;
                }

                foreach (string modelName in item.Value)
                {
                    bool exists = await _context.MotorcycleModels.AnyAsync(m => m.BrandId == brand.Id && m.Name == modelName);

                    if (!exists)
                    {
                        await _context.MotorcycleModels.AddAsync(new MotorcycleModel { Id = Guid.NewGuid(), Name = modelName, BrandId = brand.Id, IsActive = true });
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
