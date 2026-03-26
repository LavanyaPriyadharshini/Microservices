using NotificationAPI_Phase3.Models;

namespace NotificationAPI_Phase3.Services
{
    /// <summary>
    /// Service responsible for sending order notifications to customers.
    ///
    /// Current Implementation: Console logging (for learning/testing)
    /// Future Implementations: Email, SMS, Push notifications
    ///
    /// Why a separate service?
    /// - SINGLE RESPONSIBILITY: Only handles notification logic
    /// - TESTABLE: Easy to mock in unit tests
    /// - EXTENSIBLE: Easy to add email/SMS later without changing consumer
    /// - MAINTAINABLE: All notification logic in one place
    ///
    /// Service Lifecycle:
    /// - Registered as SCOPED in Program.cs
    /// - Created per Kafka message (new instance for each order)
    /// - Disposed after processing message
    /// </summary>
    public class NotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor - Dependency Injection
        /// </summary>
        /// <param name="logger">For logging notification activities</param>
        /// <param name="configuration">For accessing appsettings.json (email/SMS config later)</param>
        public NotificationService(
            ILogger<NotificationService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        /// <summary>
        /// Sends order confirmation notification to customer.
        ///
        /// CURRENT IMPLEMENTATION: Console logging only
        /// - Logs order details to console
        /// - Perfect for learning and testing Kafka flow
        /// - No external dependencies (email servers, SMS providers)
        ///
        /// FUTURE ENHANCEMENTS:
        /// 1. Send email confirmation using MailKit/SMTP
        /// 2. Send SMS using Twilio API
        /// 3. Send push notification to mobile app
        /// 4. Store notification history in database
        /// </summary>
        /// <param name="orderEvent">Order details from Kafka event</param>
        public async Task SendOrderConfirmationAsync(OrderCreatedEvent orderEvent)
        {
            // ════════════════════════════════════════════════════════════════
            // VALIDATION
            // ════════════════════════════════════════════════════════════════
            if (orderEvent == null)
            {
                _logger.LogWarning("⚠️ Received null OrderCreatedEvent, skipping notification");
                return;
            }


            // ════════════════════════════════════════════════════════════════
            // LOG NOTIFICATION DETAILS (Current Implementation)
            // ════════════════════════════════════════════════════════════════
            _logger.LogInformation(
                "═══════════════════════════════════════════════════════════════"
            );
            _logger.LogInformation(
                "📧 ORDER CONFIRMATION NOTIFICATION"
            );
            _logger.LogInformation(
                "═══════════════════════════════════════════════════════════════"
            );
            _logger.LogInformation("Order ID:        {OrderId}", orderEvent.OrderId);
            _logger.LogInformation("Customer:        {CustomerName}", orderEvent.CustomerName);
            _logger.LogInformation("Email:           {CustomerEmail}", orderEvent.CustomerEmail);
            _logger.LogInformation("Product:         {ProductName}", orderEvent.ProductName);
            _logger.LogInformation("Quantity:        {Quantity}", orderEvent.Quantity);
            _logger.LogInformation("Price per unit:  ${ProductPrice:F2}", orderEvent.ProductPrice);
            _logger.LogInformation("Total Amount:    ${TotalAmount:F2}", orderEvent.TotalAmount);
            _logger.LogInformation("Status:          {Status}", orderEvent.Status);
            _logger.LogInformation("Order Date:      {OrderDate:yyyy-MM-dd HH:mm:ss} UTC", orderEvent.OrderDate);
            _logger.LogInformation(
                "═══════════════════════════════════════════════════════════════"
            );


            // ════════════════════════════════════════════════════════════════
            // SIMULATE ASYNC OPERATION
            // ════════════════════════════════════════════════════════════════
            // In real implementation, this would be:
            // - await SendEmailAsync()
            // - await SendSmsAsync()
            // - await SaveToDatabase()
            await Task.Delay(100); // Simulate processing time


            // ════════════════════════════════════════════════════════════════
            // EMAIL NOTIFICATION (COMMENTED - Implement when needed)
            // ════════════════════════════════════════════════════════════════
            // Uncomment and implement when adding email support:
            //
            // try
            // {
            //     await SendEmailNotificationAsync(orderEvent);
            //     _logger.LogInformation("✅ Email sent to {Email}", orderEvent.CustomerEmail);
            // }
            // catch (Exception ex)
            // {
            //     _logger.LogError(ex, "❌ Failed to send email to {Email}", orderEvent.CustomerEmail);
            // }


            // ════════════════════════════════════════════════════════════════
            // SMS NOTIFICATION (COMMENTED - Implement when needed)
            // ════════════════════════════════════════════════════════════════
            // Uncomment and implement when adding SMS support:
            //
            // try
            // {
            //     await SendSmsNotificationAsync(orderEvent);
            //     _logger.LogInformation("✅ SMS sent for order {OrderId}", orderEvent.OrderId);
            // }
            // catch (Exception ex)
            // {
            //     _logger.LogError(ex, "❌ Failed to send SMS for order {OrderId}", orderEvent.OrderId);
            // }


            _logger.LogInformation(
                "✅ Notification processed successfully for Order #{OrderId}",
                orderEvent.OrderId
            );
        }


        // ════════════════════════════════════════════════════════════════════
        // PRIVATE HELPER METHODS (For future implementation)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Sends email notification using SMTP (MailKit library)
        /// FUTURE IMPLEMENTATION - Add when ready
        /// </summary>
        // private async Task SendEmailNotificationAsync(OrderCreatedEvent orderEvent)
        // {
        //     // 1. Get SMTP settings from appsettings.json
        //     var smtpServer = _configuration["Email:SmtpServer"];
        //     var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        //     var smtpUsername = _configuration["Email:SmtpUsername"];
        //     var smtpPassword = _configuration["Email:SmtpPassword"];
        //
        //     // 2. Create email message using MimeKit
        //     var message = new MimeMessage();
        //     message.From.Add(new MailboxAddress("E-Commerce", "noreply@ecommerce.com"));
        //     message.To.Add(new MailboxAddress(orderEvent.CustomerName, orderEvent.CustomerEmail));
        //     message.Subject = $"Order Confirmation #{orderEvent.OrderId}";
        //
        //     // 3. Build HTML email body
        //     message.Body = new TextPart("html")
        //     {
        //         Text = $@"
        //             <h2>Order Confirmation</h2>
        //             <p>Dear {orderEvent.CustomerName},</p>
        //             <p>Your order has been confirmed!</p>
        //             <ul>
        //                 <li><strong>Order ID:</strong> {orderEvent.OrderId}</li>
        //                 <li><strong>Product:</strong> {orderEvent.ProductName}</li>
        //                 <li><strong>Quantity:</strong> {orderEvent.Quantity}</li>
        //                 <li><strong>Total:</strong> ${orderEvent.TotalAmount:F2}</li>
        //             </ul>
        //             <p>Thank you for your purchase!</p>
        //         "
        //     };
        //
        //     // 4. Send email using MailKit SmtpClient
        //     using var client = new SmtpClient();
        //     await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
        //     await client.AuthenticateAsync(smtpUsername, smtpPassword);
        //     await client.SendAsync(message);
        //     await client.DisconnectAsync(true);
        // }


        /// <summary>
        /// Sends SMS notification using Twilio API
        /// FUTURE IMPLEMENTATION - Add when ready
        /// </summary>
        // private async Task SendSmsNotificationAsync(OrderCreatedEvent orderEvent)
        // {
        //     // 1. Get Twilio credentials from appsettings.json
        //     var accountSid = _configuration["Twilio:AccountSid"];
        //     var authToken = _configuration["Twilio:AuthToken"];
        //     var fromPhone = _configuration["Twilio:FromPhoneNumber"];
        //
        //     // 2. Initialize Twilio client
        //     TwilioClient.Init(accountSid, authToken);
        //
        //     // 3. Send SMS
        //     var message = await MessageResource.CreateAsync(
        //         body: $"Order #{orderEvent.OrderId} confirmed! {orderEvent.ProductName} x{orderEvent.Quantity}. Total: ${orderEvent.TotalAmount:F2}",
        //         from: new PhoneNumber(fromPhone),
        //         to: new PhoneNumber(orderEvent.CustomerPhoneNumber) // Add this to event
        //     );
        // }


        // ════════════════════════════════════════════════════════════════════
        // 📚 LEARNING NOTES
        // ════════════════════════════════════════════════════════════════════
        //
        // CURRENT FLOW:
        // 1. Customer creates order in Order API
        // 2. Order API publishes event to Kafka
        // 3. Kafka stores event in "order-created-topic"
        // 4. OrderKafkaConsumer (background service) consumes event
        // 5. Consumer calls NotificationService.SendOrderConfirmationAsync()
        // 6. This method logs to console (for now)
        //
        // FUTURE FLOW (with email):
        // 1-4. Same as above
        // 5. Consumer calls NotificationService.SendOrderConfirmationAsync()
        // 6. This method sends actual email via SMTP
        // 7. Customer receives email confirmation
        //
        // WHY CONSOLE LOGGING NOW?
        // - Easy to verify Kafka is working
        // - No need for email server setup
        // - Perfect for learning microservices communication
        // - Can see notifications in real-time in console
        //
        // WHEN TO ADD EMAIL/SMS?
        // - After verifying Kafka flow works perfectly
        // - When you have SMTP server credentials (Gmail/SendGrid)
        // - When you have Twilio account for SMS
        // - Simply uncomment and implement helper methods above
        //
        // ════════════════════════════════════════════════════════════════════
    }
}
