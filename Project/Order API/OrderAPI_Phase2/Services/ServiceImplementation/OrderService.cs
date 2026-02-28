using OrderAPI_Phase2.DTOs;
using OrderAPI_Phase2.Events;
using OrderAPI_Phase2.HttpClients;
using OrderAPI_Phase2.MessageBus;
using OrderAPI_Phase2.Models;
using OrderAPI_Phase2.Repositories.InterfaceRepo;
using OrderAPI_Phase2.Services.Interfaces;

namespace OrderAPI_Phase2.Services.ServiceImplementation
{
    public class OrderService(
     IOrderRepository repository,
     IProductHttpClient productHttpClient,

      IMessageBus messageBus,// ← NEW: injected via DI

     ILogger<OrderService> logger) : IOrderService

    {


     
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            logger.LogInformation("Fetching all orders");

            var orders = await repository.GetAllOrdersAsync();

            return orders.Select(MapToDto);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            logger.LogInformation("Fetching order with ID: {OrderId}", id);

            var order = await repository.GetOrderByIdAsync(id);

            if (order is null)
            {
                logger.LogWarning("Order with ID {OrderId} not found", id);
                return null;
            }

            return MapToDto(order);
        }


        //while the order is created the message will be sent to the notification api via rabbit mq

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDTO createOrderDto)
        {
            logger.LogInformation(
                "Creating order for customer {CustomerName}, Product ID: {ProductId}, Quantity: {Quantity}",
                createOrderDto.CustomerName,
                createOrderDto.ProductId,
                createOrderDto.Quantity
            );

            // Step 1: Call Product.API to get product details
            var product = await productHttpClient.GetProductByIdAsync(createOrderDto.ProductId);

            if (product is null)
            {
                logger.LogError("Product {ProductId} not found in Product.API", createOrderDto.ProductId);
                throw new InvalidOperationException($"Product with ID {createOrderDto.ProductId} does not exist");
            }

            // Step 2: Validate stock availability
            if (product.Stock < createOrderDto.Quantity)
            {
                logger.LogWarning(
                    "Insufficient stock for product {ProductId}. Available: {Stock}, Requested: {Quantity}",
                    createOrderDto.ProductId,
                    product.Stock,
                    createOrderDto.Quantity
                );
                throw new InvalidOperationException(
                    $"Insufficient stock. Available: {product.Stock}, Requested: {createOrderDto.Quantity}"
                );
            }

            // Step 3: Calculate total amount
            var totalAmount = product.Price * createOrderDto.Quantity;

            // Step 4: Create order
            var order = new OrderDetails
            {
                CustomerName = createOrderDto.CustomerName,
                CustomerEmail = createOrderDto.CustomerEmai,
                ProductId = createOrderDto.ProductId,
                Prod_Name = product.Prod_Name, // Cache product name
                ProductPrice = product.Price, // Cache price at time of order
                Quantity = createOrderDto.Quantity,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending
            };

            var createdOrder = await repository.CreateOrderAsync(order);

            logger.LogInformation(
                "Order {OrderId} created successfully. Total: {TotalAmount:C}", 
                createdOrder.Id,
                createdOrder.TotalAmount
            );


            // ── Step 5: NEW — Publish event to RabbitMQ ────────────────
            // Order is already saved to DB before we publish.
            // Why? If publish fails, order is still safe in DB.
            // We NEVER publish before saving — that would risk
            // sending a notification for an order that failed to save.


            try
            {
                var orderCreatedEvent = new OrderCreatedEvent
                {
                    OrderId = createdOrder.Id,
                    CustomerName = createdOrder.CustomerName,
                    CustomerEmail = createdOrder.CustomerEmail,
    ProductName = order.Prod_Name ?? product.Prod_Name ?? "Unknown Product", // ← use order or product directly
                    Quantity = createdOrder.Quantity,
                    TotalAmount = createdOrder.TotalAmount,
                    OrderDate = createdOrder.OrderDate,
                      PublishedAt = DateTime.UtcNow    
                };

                await messageBus.PublishOrderCreatedAsync(orderCreatedEvent); //calls the interface and implements the code in the Rabbitmqmessage bus

                logger.LogInformation(
                    "📤 OrderCreatedEvent published for Order {OrderId}",
                    createdOrder.Id);
            }

            catch (Exception ex)
            {
                // We catch but DO NOT rethrow.
                // Why? Order is already saved successfully in DB.
                // A RabbitMQ failure should NOT fail the entire order.
                // Customer should still get their order confirmed.
                // In production → use Outbox Pattern for guaranteed delivery.
                logger.LogError(ex,
                    "⚠️ Failed to publish event for Order {OrderId}. Order was saved successfully.",
                    createdOrder.Id);
            }

            return MapToDto(createdOrder);
        }






        private static OrderDto MapToDto(OrderDetails order) => new(
 order.Id,
 order.orderId,
 order.CustomerName,
 order.CustomerEmail,
 order.ProductId,
 order.Prod_Name,          // ✅ FIXED
 order.ProductPrice,
 order.Quantity,
 order.TotalAmount,
 order.Status.ToString(),
 order.OrderDate
);


    }
}