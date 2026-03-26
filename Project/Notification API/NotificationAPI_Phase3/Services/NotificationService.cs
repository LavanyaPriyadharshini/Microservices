using NotificationAPI_Phase3.Models;

namespace NotificationAPI_Phase3.Services
{
    /// <summary>
    /// Handles the actual notification logic.
    /// Consumer receives message in the below format → calls this service → logs email.
    /// this message you can see it in the kafka and rabbit mq
    /// In production: replace logging with real email (SendGrid/SMTP).
    /// </summary>
    public class NotificationService(ILogger<NotificationService> logger)
    {
        public Task SendOrderConfirmationAsync(OrderCreatedEvent orderEvent)
        {
            var processedAt = DateTime.UtcNow;

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
            logger.LogInformation("Event published at:  {PublishedAt}", orderEvent.PublishedAt);
            logger.LogInformation("Notification processed at: {ProcessedAt}", processedAt);
            logger.LogInformation("Processing delay: {Delay}ms",
                (processedAt - orderEvent.PublishedAt).TotalMilliseconds);
            logger.LogInformation("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            return Task.CompletedTask;
        }
    }
    }
