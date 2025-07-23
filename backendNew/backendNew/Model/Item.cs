using System.ComponentModel.DataAnnotations;

namespace backendNew.Model
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]

        public string? Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int Price { get; set; }
    }
}
