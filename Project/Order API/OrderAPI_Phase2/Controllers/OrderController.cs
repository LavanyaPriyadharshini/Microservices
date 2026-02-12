using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAPI_Phase2.DTOs;
using OrderAPI_Phase2.Services.Interfaces;

namespace OrderAPI_Phase2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class OrderController(IOrderService orderService, ILogger<OrderController> logger) : ControllerBase
    {
        /// <summary>
        /// Get all orders
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            logger.LogInformation("API: Getting all orders");
            var orders = await orderService.GetAllOrdersAsync();
            return Ok(orders);
        }


        /// <summary>
        /// Get order by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            logger.LogInformation("API: Getting order with ID: {OrderId}", id);

            var order = await orderService.GetOrderByIdAsync(id);

            return order is null
                ? NotFound(new { message = $"Order with ID {id} not found" })
                : Ok(order);
        }


        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDTO createOrderDto)
        {
            try
            {
                logger.LogInformation("API: Creating new order for customer: {CustomerName}", createOrderDto.CustomerName);

                var order = await orderService.CreateOrderAsync(createOrderDto);

                return CreatedAtAction(
                    nameof(GetOrder),
                    new { id = order.Id },
                    order
                );
            }
            catch (InvalidOperationException ex)
            {
                // Business logic errors (product not found, insufficient stock)
                logger.LogWarning("Order creation failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // System errors (Product.API down, network issues)
                logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { message = "An error occurred while creating the order. Please try again." });
            }

        }
    }

}
