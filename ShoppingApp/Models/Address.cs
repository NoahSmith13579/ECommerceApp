using ShoppingApp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApp.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Street may contain only alphanumeric characters")]
        public string Street { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Street2 may contain only alphanumeric characters")]
        public string? Street2 { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "City may only contain alphabetical characters")]
        public string City { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Country may only contain alphabetical characters")]
        public string Country { get; set; }
        [Required]

        [RegularExpression(@"^[0-9- ]+$", ErrorMessage = "Zipcode may contain only numeric and dash characters")]
        public string ZipCode { get; set; }
        [Required]
        [ForeignKey(nameof(UserId))]
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

    }
}
