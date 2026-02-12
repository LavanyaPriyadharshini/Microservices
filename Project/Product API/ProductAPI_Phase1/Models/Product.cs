using System.ComponentModel.DataAnnotations;

namespace ProductAPI_Phase1.Models
{
    public class Product : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public required string Prod_Name { get; set; }

        public required string Description { get; set; }

        public decimal Price { get; set; }

        // Internal field - Cost price (not exposed to customers)
        public decimal CostPrice { get; set; }

        public int Stock { get; set; }

        public required string Category { get; set; }

        public string? ProdImageUrl { get; set; }

        // Internal field - Supplier reference
        public int SupplierId { get; set; }
    }
}
