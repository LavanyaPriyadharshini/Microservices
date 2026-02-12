using System.ComponentModel.DataAnnotations;

namespace ProductAPI_Phase1.DTOs
{
    public record CreateProductDTO
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = default!;


        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; } = default!;


        [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Cost price must be greater than 0")]
        public decimal CostPrice { get; set; } = default!;


        [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; } = default!;


        [Required(ErrorMessage = "Category is required")]
    public string Category { get; set; } = default!;


        public string? ImageUrl { get; set; } = default!;


        [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Valid supplier ID is required")]
        public int SupplierId { get; set; } = default!;
    }
}
