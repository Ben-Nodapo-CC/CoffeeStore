using CoffeeStore.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeStore.Context
{
    public class CoffeeDbContext : DbContext
    {
        public CoffeeDbContext(DbContextOptions<CoffeeDbContext> options) : base (options) {}

        public DbSet<Coffee> Coffees { get; set; }

    }
}
