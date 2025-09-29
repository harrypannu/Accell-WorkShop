using System.ComponentModel.DataAnnotations;

namespace WiproPOC.Model
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(200)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        public int Age { get; set; }
    }
}
