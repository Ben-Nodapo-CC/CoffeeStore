using System.ComponentModel.DataAnnotations;

namespace CoffeeStore.Dtos
{
    public class CoffeeCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int SerialNumber { get; set; }
    }
}
