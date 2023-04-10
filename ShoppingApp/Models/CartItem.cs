using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Models
{
    public class CartItem
    {

        [Key]
        public string CartItemId { get; set; }
        public string ShoppingCartId { get; set; }
        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }


    }
}
