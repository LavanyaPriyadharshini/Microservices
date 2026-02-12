using System.ComponentModel.DataAnnotations;

namespace OrderAPI_Phase2.DTOs
{

    /// <summary>
    /// DTO for creating new order
    /// </summary>
    public record CreateOrderDTO
    {

        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string CustomerEmai { get; set; }


        [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Valid product ID is required")]
        public int ProductId { get; set; }

        [Required]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }

    }
}
