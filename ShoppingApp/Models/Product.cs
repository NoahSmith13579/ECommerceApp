using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }

        public string ImgUrl { get; set; }
        public Category Category { get; set; }

    }
}
