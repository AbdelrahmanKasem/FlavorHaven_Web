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

        // GET: api/Order
        [HttpGet("GetOrders")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .ToListAsync();

            return orders.Select(o => MapToOrderDto(o)).ToList();
        }

        // GET: api/Order/5
        [HttpGet("GetOrder/{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return MapToOrderDto(order);
        }

        // POST: api/Order
        //[HttpPost("CreateOrder")]
        //public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        ////public async Task<ActionResult<OrderDto>> CreateOrder()
        //{

        //    if (!Enum.TryParse<OrderType>(createOrderDto.Type, out var orderType))
        //    {
        //        return BadRequest("Invalid order type");
        //    }

        //    var order = new Order
        //    {
        //        Id = Guid.NewGuid(),
        //        OrderDate = DateTime.UtcNow,
        //        Status = OrderStatus.Pending,
        //        Type = orderType,
        //        PaymentSystem = createOrderDto.PaymentSystem,
        //        Note = createOrderDto.Note,
        //        CustomerId = createOrderDto.CustomerId,
        //        OrderItems = new List<OrderItem>()
        //    };

        //    decimal totalPrice = 0;

        //    foreach (var itemDto in createOrderDto.OrderItems)
        //    {
        //        if (!Enum.TryParse<SpicyLevel>(itemDto.SpicyLevel, out var spicyLevel))
        //        {
        //            return BadRequest("Invalid spicy level");
        //        }

        //        var menuItem = await _context.MenuItems.FindAsync(itemDto.MenuItemId);
        //        if (menuItem == null)
        //        {
        //            return BadRequest($"MenuItem with id {itemDto.MenuItemId} not found");
        //        }

        //        var orderItem = new OrderItem
        //        {
        //            Id = Guid.NewGuid(),
        //            Quantity = itemDto.Quantity,
        //            Note = itemDto.Note,
        //            SpicyLevel = spicyLevel,
        //            Price = menuItem.Price
        //        };

        //        // Calculate price with customizations
        //        decimal itemPrice = menuItem.Price * itemDto.Quantity;
        //        foreach (var customizationDto in itemDto.Customizations)
        //        {
        //            orderItem.Customizations.Add(new OrderItemCustomization
        //            {
        //                Id = Guid.NewGuid(),
        //                Name = customizationDto.Name,
        //                ExtraPrice = customizationDto.ExtraPrice,
        //                OrderItemId = orderItem.Id
        //            });
        //            itemPrice += customizationDto.ExtraPrice * itemDto.Quantity;
        //        }

        //        orderItem.Price = itemPrice;
        //        totalPrice += itemPrice;

        //        order.OrderItems.Add(orderItem);
        //    }

        //    order.Price = totalPrice;

        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    // Send real-time notification to chefs
        //    var orderDto = MapToOrderDto(order);
        //    await _hubContext.Clients.All.SendAsync("ReceiveNewOrder", "This is just a message");

        //    //return Ok();

        //    return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);

        //    ////return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, MapToOrderDto(order));
        //}

        //[HttpPost("CreateOrder")]
        //public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        //{
        //    if (!Enum.TryParse<OrderType>(createOrderDto.Type, out var orderType))
        //    {
        //        return BadRequest("Invalid order type");
        //    }

        //    var order = new Order
        //    {
        //        Id = Guid.NewGuid(),
        //        OrderDate = DateTime.UtcNow,
        //        Status = OrderStatus.Pending,
        //        Type = orderType,
        //        PaymentSystem = createOrderDto.PaymentSystem,
        //        Note = createOrderDto.Note,
        //        CustomerId = createOrderDto.CustomerId,
        //        OrderItems = new List<OrderItem>()
        //    };

        //    decimal totalPrice = 0;

        //    foreach (var itemDto in createOrderDto.OrderItems)
        //    {
        //        if (!Enum.TryParse<SpicyLevel>(itemDto.SpicyLevel, out var spicyLevel))
        //        {
        //            return BadRequest("Invalid spicy level");
        //        }

        //        var menuItem = await _context.MenuItems.FindAsync(itemDto.MenuItemId);
        //        if (menuItem == null)
        //        {
        //            return BadRequest($"MenuItem with id {itemDto.MenuItemId} not found");
        //        }

        //        var orderItem = new OrderItem
        //        {
        //            Id = Guid.NewGuid(),
        //            Quantity = itemDto.Quantity,
        //            Note = itemDto.Note,
        //            SpicyLevel = spicyLevel,
        //            Price = menuItem.Price
        //        };

        //        // Calculate price without customizations
        //        decimal itemPrice = menuItem.Price * itemDto.Quantity;

        //        orderItem.Price = itemPrice;
        //        totalPrice += itemPrice;

        //        order.OrderItems.Add(orderItem);
        //    }

        //    order.Price = totalPrice;

        //    order.EstimatedPreparationTime = TimeSpan.FromMinutes(order.OrderItems.Sum(item => item.Quantity) * 3);

        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    // Send real-time notification to chefs
        //    var orderDto = MapToOrderDto(order);
        //    await _hubContext.Clients.All.SendAsync("ReceiveNewOrder", "This is just a message");

        //    return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
        //}

        [HttpPost("PlaceOrderFromCart")]
        public async Task<ActionResult<OrderDto>> PlaceOrderFromCart(Guid userId, string paymentSystem, string? note, string type)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

            if (cart == null || !cart.Items.Any())
                return BadRequest("Cart is empty or not found.");

            if (!Enum.TryParse<OrderType>(type, out var orderType))
                return BadRequest("Invalid order type");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Type = orderType,
                PaymentSystem = paymentSystem,
                Note = note,
                CustomerId = userId,
                OrderItems = new List<OrderItem>(),
                Price = cart.TotalPrice,
                EstimatedPreparationTime = TimeSpan.FromMinutes(cart.Items.Sum(i => i.Quantity) * 3)
            };

            foreach (var cartItem in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    Quantity = cartItem.Quantity,
                    Note = "", // Could add optional note per item in CartItem if needed
                    SpicyLevel = SpicyLevel.Medium, // Default or fetch if stored in cart
                    Price = cartItem.PriceAtTimeOfOrder,
                    MenuItemId = cartItem.MenuItemId
                };

                order.OrderItems.Add(orderItem);
            }

            // Mark the cart as checked out
            cart.IsCheckedOut = true;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderDto = MapToOrderDto(order);
            await _hubContext.Clients.All.SendAsync("ReceiveNewOrder", orderDto);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
        }

        // PUT: api/Order/5/status
        [HttpPut("Order/{id}/Status")]
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

            await _hubContext.Clients.Group("Chefs").SendAsync("OrderStatusUpdated",
            new { OrderId = id, NewStatus = order.Status.ToString() });

            return NoContent();
        }

        // DELETE: api/Order/5
        [HttpDelete("DeleteOrder/{id}")]
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