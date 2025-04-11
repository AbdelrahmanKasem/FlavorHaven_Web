// Controllers/OrderController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.DTOs;
using RMSProjectAPI.Hubs;
using RMSProjectAPI.Model;
using System.Net;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderController(AppDbContext context, IHubContext<OrderHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;

        }

        [HttpPost("CreateOrder")]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            // Validate the Order Type
            if (!Enum.TryParse<OrderType>(createOrderDto.Type.ToString(), out var orderType))
                return BadRequest("Invalid order type");

            // Create the Order object
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Type = orderType,
                Latitude = createOrderDto.Latitude,
                Longitude = createOrderDto.Longitude,
                Address = createOrderDto.Address,
                PaymentSystem = createOrderDto.PaymentSystem,
                TransactionId = createOrderDto.TransactionId,
                Note = createOrderDto.Note,
                CustomerId = createOrderDto.CustomerId,
                OrderItems = new List<OrderItem>()
            };

            decimal totalPrice = 0;
            TimeSpan totalTime = TimeSpan.Zero;
            foreach (var itemDto in createOrderDto.OrderItems)
            {
                // Retrieve the MenuItem and MenuItemSize
                var menuItem = await _context.MenuItems
                    .Include(mi => mi.Sizes)  // Ensure you load the Sizes of the MenuItem
                    .FirstOrDefaultAsync(mi => mi.Id == itemDto.MenuItemId);

                if (menuItem == null)
                    return BadRequest($"MenuItem with ID {itemDto.MenuItemId} not found");

                // Find the specific MenuItemSize based on the given MenuItemSizeId
                var menuItemSize = menuItem.Sizes.FirstOrDefault(ms => ms.Id == itemDto.MenuItemSizeId);
                totalTime += menuItem.Duration;
                if (menuItemSize == null)
                    return BadRequest($"MenuItemSize with ID {itemDto.MenuItemSizeId} not found for MenuItem with ID {itemDto.MenuItemId}");

                // Create the OrderItem
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    Quantity = itemDto.Quantity,
                    Note = itemDto.Note,
                    SpicyLevel = itemDto.SpicyLevel,
                    Price = menuItemSize.Price * itemDto.Quantity,  // Calculate price based on size and quantity
                    MenuItemId = menuItem.Id,
                    MenuItemSizeId = menuItemSize.Id  // Link the size to the OrderItem
                };

                totalPrice += orderItem.Price;
                order.OrderItems.Add(orderItem);
            }

            // Set the total price and estimated preparation time
            order.Price = totalPrice;
            order.EstimatedPreparationTime = totalTime; // Example, can be adjusted based on menu item or other factors
            // Add the order to the database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Map to OrderDto and return the result
            var orderDto = MapToOrderDto(order);
            orderDto.EstimatedPreparationTime = totalTime;

            // SignalR: Notify chefs
            await _hubContext.Clients.All.SendAsync("ReceiveNewOrder", orderDto);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
        }

        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderDtos = orders.Select(o => MapToOrderDto(o)).ToList();
            return Ok(orderDtos);
        }

        [HttpGet("GetOrder/{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Order not found");

            return Ok(MapToOrderDto(order));
        }

        [HttpPut("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound("Order not found");

            if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var status))
                return BadRequest("Invalid status");

            order.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteOrder/{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Order not found");

            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("CustomerOrders/{customerId}")]
        public async Task<ActionResult<List<OrderDto>>> GetOrdersByCustomer(Guid customerId)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderDtos = orders.Select(o => MapToOrderDto(o)).ToList();
            return Ok(orderDtos);
        }

        [HttpGet("FilterOrders")]
        public async Task<ActionResult<List<OrderDto>>> FilterOrders([FromQuery] string? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = _context.Orders.Include(o => o.OrderItems).AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
                query = query.Where(o => o.Status == orderStatus);

            if (from.HasValue)
                query = query.Where(o => o.OrderDate >= from.Value);

            if (to.HasValue)
                query = query.Where(o => o.OrderDate <= to.Value);

            var orders = await query.ToListAsync();
            var orderDtos = orders.Select(o => MapToOrderDto(o)).ToList();

            return Ok(orderDtos);
        }

        private OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Type = order.Type,
                Price = order.Price,
                PaymentSystem = order.PaymentSystem,
                TransactionId = order.TransactionId,
                Note = order.Note,
                CustomerId = order.CustomerId,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    Quantity = oi.Quantity,
                    Note = oi.Note,
                    SpicyLevel = oi.SpicyLevel,
                    Price = oi.Price,
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = _context.MenuItems.FirstOrDefault(mi => mi.Id == oi.MenuItemId)?.Name ?? "Unknown"
                }).ToList()
            };
        }

        [HttpGet("GetOrderTime/{id}")]
        public async Task<ActionResult<OrderTimeDto>> GetOrderTime(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound("Order not found");

            var dto = new OrderTimeDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                EstimatedPreparationTime = order.EstimatedPreparationTime
            };

            return Ok(dto);
        }
    }
}