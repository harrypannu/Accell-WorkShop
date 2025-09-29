using System.ComponentModel.DataAnnotations;

namespace WiproPOC_OrderApi.Model
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }
}
