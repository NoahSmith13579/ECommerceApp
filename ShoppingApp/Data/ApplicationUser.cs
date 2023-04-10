using Microsoft.AspNetCore.Identity;
using ShoppingApp.Models;

namespace ShoppingApp.Data
{

    public class ApplicationUser : IdentityUser
    {
        public Address Address { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public List<Order> Orders { get; set; }
    }
}

