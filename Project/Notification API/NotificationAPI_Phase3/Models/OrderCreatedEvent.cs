namespace NotificationAPI_Phase3.Models
{
    public class OrderCreatedEvent
    {
        /// <summary>
        /// Must match EXACTLY with Order.API's OrderCreatedEvent.
        /// 
        /// refer the order api, the names give there in the order api model class must match here(in this notification model)  accurately 
        /// Same property names and types on both sides.
        /// Order.API serializes → RabbitMQ/Kafka stores → 
        /// Notification.API deserializes back to this class.
        /// </summary>
       
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

