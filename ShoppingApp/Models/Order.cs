using ShoppingApp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApp.Models
{
    public class Order
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public int AddressId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }


        public List<OrderItem> OrderItems { get; set; }
        public ApplicationUser User { get; set; }
        public Address Address { get; set; }
    }
}
