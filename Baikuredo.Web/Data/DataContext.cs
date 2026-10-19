using Microsoft.EntityFrameworkCore;
using Baikuredo.Web.Data.Entities;

namespace Baikuredo.Web.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
    }
}
