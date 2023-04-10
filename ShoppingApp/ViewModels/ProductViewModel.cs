using ShoppingApp.Models;

namespace ShoppingApp.ViewModels
{
    public class ProductViewModel
    {
        public List<Product> Products { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
