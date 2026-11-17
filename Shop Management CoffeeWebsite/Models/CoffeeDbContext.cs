using System.Data.Entity;

namespace CafeManagement.Models
{
    public class CoffeeDbContext : DbContext
    {
        public CoffeeDbContext() : base("name=MimiCoffeeDB")
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}