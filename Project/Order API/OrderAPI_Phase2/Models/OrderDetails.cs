using System.ComponentModel.DataAnnotations;

namespace OrderAPI_Phase2.Models
{
    public class OrderDetails : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int orderId { get; set; }
        public required string CustomerName { get; set; } = string.Empty;

        public required string CustomerEmail { get; set; } = string.Empty;

        public int ProductId { get; set; }

        public required string Prod_Name { get; set; } = string.Empty; // Cached from Product.API

        public decimal ProductPrice { get; set; } // Price at time of order

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; } // ProductPrice * Quantity


        // OrderStatus is the TYPE (like int, string, etc.)
        // Status is the PROPERTY name
        // OrderStatus.Pending is the DEFAULT value
        public OrderStatus Status { get; set; } = OrderStatus.Pending; //default is pending

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Order status enum, 
    /// refer the explanation of enum in the microservices intro word file at page number 7
    /// </summary>
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Shipped,
        Delivered,
        Cancelled
    }
}
