using NotificationAPI_Phase3.Models;

namespace NotificationAPI_Phase3.Services
{

    /// <summary>
    /// Handles the actual notification logic.
    /// 
    /// Why separate from the consumer?
    /// Consumer's job  → receive message from RabbitMQ
    /// Service's job   → decide what to DO with that message
    /// 
    /// Same separation of concerns pattern you used in Phase 1 and 2:
    /// Controller → Service → Repository
    /// Consumer   → Service (here)
    /// 
    /// In production this would send real emails via SendGrid/SMTP.
    /// For learning, we simulate it with detailed logging.
    /// </summary>
    /// 

    public class NotificationService(ILogger<NotificationService> logger)
    {
        public Task SendOrderConfirmationAsync(OrderCreatedEvent orderEvent)
        {
            // Simulate sending an order confirmation email
            // In production: replace this with SendGrid or SMTP email code
            //so if the property names from the order api matches then only it can show the corrct values when a user places the order
            logger.LogInformation("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            logger.LogInformation("📧 ORDER CONFIRMATION EMAIL");
            logger.LogInformation("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            logger.LogInformation("To:       {CustomerEmail}", orderEvent.CustomerEmail);
            logger.LogInformation("Subject:  Order #{OrderId} Confirmed!", orderEvent.OrderId);
            logger.LogInformation("───────────────────────────────────────────");
            logger.LogInformation("Dear {CustomerName},", orderEvent.CustomerName);
            logger.LogInformation("Your order has been confirmed successfully.");
            logger.LogInformation("Product:  {ProductName}", orderEvent.ProductName);
            logger.LogInformation("Quantity: {Quantity}", orderEvent.Quantity);
            logger.LogInformation("Total:    {TotalAmount:C}", orderEvent.TotalAmount);
            logger.LogInformation("Date:     {OrderDate}", orderEvent.OrderDate);
            logger.LogInformation("───────────────────────────────────────────");
            var processedAt = DateTime.UtcNow;
            logger.LogInformation("Event published at:  {PublishedAt}", orderEvent.PublishedAt);
            logger.LogInformation("Notification processed at: {ProcessedAt}", processedAt);
            logger.LogInformation("Processing delay: {Delay}ms",
                (processedAt - orderEvent.PublishedAt).TotalMilliseconds);
            logger.LogInformation("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            return Task.CompletedTask;
        }
    }
}
