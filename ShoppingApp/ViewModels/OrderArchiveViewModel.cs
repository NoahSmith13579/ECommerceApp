
using ShoppingApp.Helper;
using ShoppingApp.Models;
namespace ShoppingApp.ViewModels
{
    public class OrderArchiveViewModel
    {
        public PagedList<Order> Orders { get; set; }

        //List<OrderItem> OrderItems { get; set; }
    }
}
