using ShoppingApp.Data;

namespace ShoppingApp.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string? Street2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
