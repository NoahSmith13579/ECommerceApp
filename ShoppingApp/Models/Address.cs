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
        [RegularExpression(@"^([a-zA-Z\u0080-\u024F]+(?:. |-| |'))*[a-zA-Z\u0080-\u024F]*$", ErrorMessage = "City may not contain special characters")]
        public string City { get; set; }
        [Required]
        [RegularExpression(@"^([a-zA-Z\u0080-\u024F]+(?:. |-| |'))*[a-zA-Z\u0080-\u024F]*$", ErrorMessage = "Country may not contain special characters")]
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
