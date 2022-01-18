using CoffeeStore.Context;
using CoffeeStore.Models;

namespace CoffeeStore.Repositories
{
    public class CoffeeRepo : ICoffeeRepo
    {
        private readonly CoffeeDbContext _context;

        public CoffeeRepo(CoffeeDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Coffee> GetCoffees()
        {
            return _context.Coffees
                .Where(c => !c.IsInCheckout)
                .ToList();
        }

        public Coffee? GetCoffeeById(int id) => 
            _context.Coffees.FirstOrDefault(c => c.Id == id);

        public void CreateCoffee(Coffee coffee)
        {
           if(coffee == null)
                throw new ArgumentNullException(nameof(coffee));

           _context.Coffees.Add(coffee);
           SaveChanges();
        }

        public Coffee? UpdateCoffeeForCheckout(int id)
        {
            var coffeeToUpdate = GetCoffeeById(id);

            if (coffeeToUpdate != null)
            {
                coffeeToUpdate.IsInCheckout = !coffeeToUpdate.IsInCheckout;
                coffeeToUpdate.LastUpdated = DateTime.UtcNow;
                _context.Coffees.Update(coffeeToUpdate);
                SaveChanges();

            }
            return coffeeToUpdate;
        }

        public bool DeleteCoffee(int id)
        {
            var coffeeToDelete = GetCoffeeById(id);
            
            if(coffeeToDelete != null)
            {
                _context.Coffees.Remove(coffeeToDelete);
                return SaveChanges();
            }
            
             return false; 

        }

        private bool SaveChanges() => 
            _context.SaveChanges() >= 0;

    }
}
