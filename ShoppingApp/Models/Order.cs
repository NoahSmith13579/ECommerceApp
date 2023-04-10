using ShoppingApp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApp.Models
{
    public class Order
    {
        [Required]
        public int Id { get; set; }

        public string ShoppingCartId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public int AddressId { get; set; }


        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        public ApplicationUser User { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public Address Address { get; set; }


    }
}
