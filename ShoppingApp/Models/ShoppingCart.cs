using ShoppingApp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApp.Models
{
    public partial class ShoppingCart
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public List<CartItem> CartItems { get; set; }
        public ApplicationUser User { get; set; }
    }
}
