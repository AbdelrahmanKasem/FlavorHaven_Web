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
            if (!Enum.TryParse<OrderType>(createOrderDto.Type.ToString(), out var orderType))
                return BadRequest("Invalid order type");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Paid,
                Type = orderType,
                Latitude = createOrderDto.Latitude,
                Longitude = createOrderDto.Longitude,
                Address = createOrderDto.Address,
                PaymentSystem = createOrderDto.PaymentSystem,
                TransactionId = createOrderDto.TransactionId,
                DeliveryFee = (decimal)createOrderDto.DeliveryFee,
                Note = createOrderDto.Note,
                CustomerId = createOrderDto.CustomerId,
                DeliveryId = createOrderDto.DeliveryId,
                WaiterId = createOrderDto.WaiterId,
                TableId = createOrderDto.TableId,
                OrderItems = new List<OrderItem>()
            };

            decimal totalPrice = 0;
            TimeSpan totalTime = TimeSpan.Zero;
            foreach (var itemDto in createOrderDto.OrderItems)
            {
                var menuItem = await _context.MenuItems
                    .Include(mi => mi.Sizes)
                    .FirstOrDefaultAsync(mi => mi.Id == itemDto.MenuItemId);

                if (menuItem == null)
                    return BadRequest($"MenuItem with ID {itemDto.MenuItemId} not found");

                var menuItemSize = menuItem.Sizes.FirstOrDefault(ms => ms.Id == itemDto.MenuItemSizeId);
                totalTime += menuItem.Duration;
                if (menuItemSize == null)
                    return BadRequest($"MenuItemSize with ID {itemDto.MenuItemSizeId} not found for MenuItem with ID {itemDto.MenuItemId}");

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    Quantity = itemDto.Quantity,
                    Note = itemDto.Note,
                    SpicyLevel = itemDto.SpicyLevel,
                    Price = menuItemSize.Price * itemDto.Quantity,
                    MenuItemId = menuItem.Id,
                    MenuItemSizeId = menuItemSize.Id 
                };

                totalPrice += orderItem.Price;
                order.OrderItems.Add(orderItem);
            }

            order.Price = totalPrice + (decimal)(createOrderDto.DeliveryFee ?? 0);
            order.EstimatedPreparationTime = totalTime;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderDto = MapToOrderDto(order);
            orderDto.EstimatedPreparationTime = totalTime;

            // SignalR
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

        [HttpGet("GetGroupedActiveOrders")]
        public async Task<ActionResult<Dictionary<OrderStatus, List<OrderDto>>>> GetGroupedActiveOrders()
        {
            var today = DateTime.UtcNow.Date;

            var filteredOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o =>
                    o.Status == OrderStatus.Paid ||
                    o.Status == OrderStatus.InProgress ||
                    (o.Status == OrderStatus.Ready))
                .ToListAsync();

            var groupedOrders = filteredOrders
                .GroupBy(o => o.Status)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(o => MapToOrderDto(o)).ToList()
                );

            return Ok(groupedOrders);
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
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(Guid id, UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Order not found");

            if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var status))
                return BadRequest("Invalid status");

            order.Status = status;
            await _context.SaveChangesAsync();

            var orderDto = MapToOrderDto(order);

            // SignalR - notify clients about the updated order status
            await _hubContext.Clients.All.SendAsync("OrderStatusUpdated", orderDto);

            return Ok(orderDto);
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
                DeliveryFee = order.DeliveryFee,
                Latitude = order.Latitude,
                Longitude = order.Longitude,
                Address = order.Address,
                PaymentSystem = order.PaymentSystem,
                TransactionId = order.TransactionId,
                Note = order.Note,
                EstimatedPreparationTime = order.EstimatedPreparationTime,
                CustomerId = order.CustomerId,
                DeliveryId = order.DeliveryId,
                WaiterId = order.WaiterId,
                TableId = order.TableId,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    Quantity = oi.Quantity,
                    Note = oi.Note,
                    SpicyLevel = oi.SpicyLevel,
                    Price = oi.Price,
                    MenuItemId = oi.MenuItemId,
                    MenuItemSizeId = oi.MenuItemSizeId,
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

        [HttpGet("GetReadyDeliveryOrders")]
        public async Task<ActionResult<List<OrderDto>>> GetReadyDeliveryOrders()
        {
            var readyDeliveryOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Ready && o.Type == OrderType.Delivery)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderDtos = readyDeliveryOrders.Select(o => MapToOrderDto(o)).ToList();

            return Ok(orderDtos);
        }

        [HttpGet("GetOrdersByDelivery/{deliveryId}")]
        public async Task<ActionResult<List<OrderDto>>> GetOrdersByDelivery(Guid deliveryId)
        {
            var orders = await _context.Orders
                .Where(o => o.DeliveryId == deliveryId)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderDtos = orders.Select(o => MapToOrderDto(o)).ToList();

            return Ok(orderDtos);
        }

        // ====================== For Waiter ========================
        [HttpGet("GetReadyWaiterOrders")]
        public async Task<ActionResult<List<OrderDto>>> GetReadyWaiterOrders()
        {
            var readyDeliveryOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Ready && o.Type == OrderType.DineIn)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderDtos = readyDeliveryOrders.Select(o => MapToOrderDto(o)).ToList();

            return Ok(orderDtos);
        }
    }
}