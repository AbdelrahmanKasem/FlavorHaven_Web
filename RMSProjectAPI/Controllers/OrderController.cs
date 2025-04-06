// Controllers/OrderController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.DTOs;
using System.Net;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Customizations)
                .Include(o => o.Customer)
                .ToListAsync();

            return orders.Select(o => MapToOrderDto(o)).ToList();
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Customizations)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return MapToOrderDto(order);
        }

        // POST: api/Order
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            if (!Enum.TryParse<OrderType>(createOrderDto.Type, out var orderType))
            {
                return BadRequest("Invalid order type");
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Type = orderType,
                PaymentSystem = createOrderDto.PaymentSystem,
                Note = createOrderDto.Note,
                CustomerId = createOrderDto.CustomerId,
                OrderItems = new List<OrderItem>()
            };

            decimal totalPrice = 0;

            foreach (var itemDto in createOrderDto.OrderItems)
            {
                if (!Enum.TryParse<SpicyLevel>(itemDto.SpicyLevel, out var spicyLevel))
                {
                    return BadRequest("Invalid spicy level");
                }

                var menuItem = await _context.MenuItems.FindAsync(itemDto.MenuItemId);
                if (menuItem == null)
                {
                    return BadRequest($"MenuItem with id {itemDto.MenuItemId} not found");
                }

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    Quantity = itemDto.Quantity,
                    Note = itemDto.Note,
                    SpicyLevel = spicyLevel,
                    Price = menuItem.Price,
                    Customizations = new List<OrderItemCustomization>()
                };

                // Calculate price with customizations
                decimal itemPrice = menuItem.Price * itemDto.Quantity;
                foreach (var customizationDto in itemDto.Customizations)
                {
                    orderItem.Customizations.Add(new OrderItemCustomization
                    {
                        Id = Guid.NewGuid(),
                        Name = customizationDto.Name,
                        ExtraPrice = customizationDto.ExtraPrice,
                        OrderItemId = orderItem.Id
                    });
                    itemPrice += customizationDto.ExtraPrice * itemDto.Quantity;
                }

                orderItem.Price = itemPrice;
                totalPrice += itemPrice;

                order.OrderItems.Add(orderItem);
            }

            order.Price = totalPrice;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, MapToOrderDto(order));
        }

        // PUT: api/Order/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, UpdateOrderStatusDto updateOrderStatusDto)
        {
            if (!Enum.TryParse<OrderStatus>(updateOrderStatusDto.Status, out var orderStatus))
            {
                return BadRequest("Invalid order status");
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = orderStatus;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                Type = order.Type.ToString(),
                Price = order.Price,
                PaymentSystem = order.PaymentSystem,
                Note = order.Note,
                CustomerId = order.CustomerId,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    Quantity = oi.Quantity,
                    Note = oi.Note,
                    SpicyLevel = oi.SpicyLevel.ToString(),
                    Price = oi.Price,
                    MenuItemId = oi.MenuItemId,
                    MenuItemName = _context.MenuItems.FirstOrDefault(mi => mi.Id == oi.MenuItemId)?.Name ?? "Unknown",
                    Customizations = oi.Customizations.Select(c => new OrderItemCustomizationDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ExtraPrice = c.ExtraPrice
                    }).ToList()
                }).ToList()
            };
        }
    }
}