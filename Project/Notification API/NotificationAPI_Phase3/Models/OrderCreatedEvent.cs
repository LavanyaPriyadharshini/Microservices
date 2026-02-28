namespace NotificationAPI_Phase3.Models
{


    /// <summary>
    /// This is the SAME shape as Order.API's OrderCreatedEvent.
    /// Both sides must match exactly — same property names and types.
    /// 
    /// Order.API     → serializes this to JSON → sends to RabbitMQ
    /// Notification.API → receives JSON → deserializes back to this class
    /// 
    /// If property names don't match → deserialization fails → null values
    /// </summary>

    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
