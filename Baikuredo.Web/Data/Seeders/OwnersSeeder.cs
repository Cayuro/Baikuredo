using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Data.Entities;

namespace Baikuredo.Web.Data.Seeders
{
    public class OwnersSeeder
    {
        private readonly DataContext _context;

        public OwnersSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            List<Owner> owners = new List<Owner>
            {
                new Owner { Id = Guid.NewGuid(), DocumentType = "CC", Document = "1000000001", FirstName = "Carlos", LastName = "Gómez", Phone = "3001112233", Email = "carlos.gomez@correo.com", Address = "Calle 10 # 20-30, Medellín" },
                new Owner { Id = Guid.NewGuid(), DocumentType = "CC", Document = "1000000002", FirstName = "Laura", LastName = "Restrepo", Phone = "3014445566", Email = "laura.restrepo@correo.com", Address = "Carrera 45 # 12-08, Envigado" },
                new Owner { Id = Guid.NewGuid(), DocumentType = "NIT", Document = "900123456", FirstName = "Motos del Valle", LastName = "S.A.S.", Phone = "6044001122", Email = "ventas@motosdelvalle.com" },
            };

            foreach (Owner owner in owners)
            {
                bool exists = await _context.Owners.AnyAsync(o => o.DocumentType == owner.DocumentType && o.Document == owner.Document);

                if (!exists)
                {
                    await _context.Owners.AddAsync(owner);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
