namespace OrderAPI_Phase2.Events
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    }
}
