using SharedModels.Enums;
using System.ComponentModel.DataAnnotations;

namespace CoffeeStore.Models
{
    public class Coffee
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Price { get; set; }
        
        [Required]
        public bool IsInCheckout { get; set; }

        [Required]
        public int SerialNumber  { get; set; }
        
        [Required]
        public DateTime LastUpdated { get; set; }

        public Coffee()
        {
            LastUpdated = DateTime.UtcNow;
        }
    }
}
