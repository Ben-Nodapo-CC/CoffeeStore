namespace CoffeeStore.Dtos
{
    public class CoffeeReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public int Price { get; set; }

        public bool IsInCheckout { get; set; }

        public int SerialNumber { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
