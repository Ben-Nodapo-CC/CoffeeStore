using CoffeeStore.Models;

namespace CoffeeStore.Repositories
{
    public interface ICoffeeRepo
    {
        IEnumerable<Coffee> GetCoffees();
        Coffee? GetCoffeeById(int id);
        void CreateCoffee(Coffee coffee);
        Coffee? UpdateCoffeeForCheckout(int id);
        bool DeleteCoffee(int id);
    }
}
