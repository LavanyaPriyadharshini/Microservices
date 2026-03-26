namespace NotificationAPI_Phase3.Models
{
    /// <summary>
    /// Event model representing an order creation event consumed from Kafka.
    ///
    /// ⚠️ IMPORTANT: This class MUST match OrderCreatedEvent in Order API exactly!
    ///
    /// Why must they match?
    /// - Order API publishes JSON to Kafka
    /// - Notification API deserializes JSON back to object
    /// - Property names and types MUST be identical
    /// - If they don't match → deserialization fails → events lost
    ///
    /// JSON Serialization Flow:
    /// 1. Order API: C# object → JSON string → Kafka
    /// 2. Kafka: Stores JSON string in topic
    /// 3. Notification API: Kafka → JSON string → C# object
    ///
    /// Example JSON in Kafka:
    /// {
    ///   "OrderId": 123,
    ///   "CustomerName": "John Doe",
    ///   "CustomerEmail": "john@example.com",
    ///   "ProductId": 5,
    ///   "ProductName": "iPhone 15",
    ///   "ProductPrice": 999.99,
    ///   "Quantity": 2,
    ///   "TotalAmount": 1999.98,
    ///   "OrderDate": "2026-03-24T10:30:00Z",
    ///   "Status": "Pending"
    /// }
    /// </summary>
    public class OrderCreatedEvent
    {
        /// <summary>
        /// Unique order ID from Order API database
        /// Used to identify which order this notification is for
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Customer's full name
        /// Used in notification message: "Dear John, your order..."
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Customer's email address
        /// Will be used when implementing email notifications
        /// Example: Send confirmation email to this address
        /// </summary>
        public string CustomerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Product ID from Product API
        /// Reference to which product was ordered
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Product name at time of order
        /// Example: "iPhone 15 Pro Max 256GB"
        /// Used in notification: "Your order for iPhone 15..."
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Price per unit at time of order
        /// Stored for reference (prices may change after order)
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// Number of items ordered
        /// Example: Customer ordered 2 laptops
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Total order amount = ProductPrice × Quantity
        /// Example: $999.99 × 2 = $1,999.98
        /// Displayed in notification
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// When the order was created (UTC timezone)
        /// Used for tracking and displaying order date
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Current order status
        /// Example values: "Pending", "Confirmed", "Shipped", "Delivered"
        /// </summary>
        public string Status { get; set; } = "Pending";


        // ────────────────────────────────────────────────────────────────
        // 📋 HOW THIS WORKS WITH KAFKA:
        // ────────────────────────────────────────────────────────────────
        // 1. Order API creates order → publishes JSON to Kafka
        // 2. Kafka stores message in "order-created-topic"
        // 3. OrderKafkaConsumer receives JSON message
        // 4. JsonSerializer.Deserialize<OrderCreatedEvent>() converts JSON → this object
        // 5. Object passed to NotificationService.SendOrderConfirmationAsync()
        // 6. NotificationService uses properties to format notification
        // ────────────────────────────────────────────────────────────────
    }
}
