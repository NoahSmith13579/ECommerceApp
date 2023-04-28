using ShoppingApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.ViewModels
{
    public class OrderViewModel
    {
        public ShoppingCart? ShoppingCart { get; set; }
        public List<CartItem>? CartItems { get; set; }
        public List<Address>? Addresses { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public int AddressId { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }


    }
}
