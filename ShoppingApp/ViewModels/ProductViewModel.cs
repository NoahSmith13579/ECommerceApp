using ShoppingApp.Helper;
using ShoppingApp.Models;
namespace ShoppingApp.ViewModels
{
    public class ProductViewModel
    {
        public PagedList<Product> Products { get; set; }
        public List<CartItem> CartItems { get; set; }
        public List<Category> Categories { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
