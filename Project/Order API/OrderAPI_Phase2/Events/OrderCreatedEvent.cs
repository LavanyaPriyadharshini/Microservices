namespace OrderAPI_Phase2.Events
{
    /// <summary>
    /// This is the MESSAGE that Order.API sends to RabbitMQ.
    /// Notification.API will receive this exact shape.
    /// Think of it as an envelope — it contains all the data
    /// the receiver needs to process the notification.
    /// the same model you will be creating in the notification api
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

        // Timestamp of when this event was published
        // Useful for debugging and tracking delays
        public DateTime PublishedAt { get; set; } = DateTime.Now;
    }
}
