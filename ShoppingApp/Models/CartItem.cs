using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Models
{
    public class CartItem
    {

        [Key]
        [Required]
        public string CartItemId { get; set; }
        [Required]
        public string ShoppingCartId { get; set; }
        public int Quantity { get; set; }

        public string Name { get; set; }

        public decimal UnitPrice { get; set; }

        public string ImgUrl { get; set; }

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }


    }
}
