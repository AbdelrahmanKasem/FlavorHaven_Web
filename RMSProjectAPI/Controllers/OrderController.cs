using EllipticCurve.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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
        [Authorize]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            if (!Enum.TryParse<OrderType>(createOrderDto.Type.ToString(), out var orderType))
                return BadRequest("Invalid order type");

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
                    .Include(mi => mi.Extras)
                    .Include(mi => mi.Offers)
                    .FirstOrDefaultAsync(mi => mi.Id == itemDto.MenuItemId);

                if (menuItem == null)
                    return BadRequest($"MenuItem with ID {itemDto.MenuItemId} not found");

                var menuItemSize = menuItem.Sizes.FirstOrDefault(ms => ms.Id == itemDto.MenuItemSizeId);
                if (menuItemSize == null)
                    return BadRequest($"MenuItemSize with ID {itemDto.MenuItemSizeId} not found for MenuItem with ID {itemDto.MenuItemId}");

                totalTime += menuItem.Duration;

                var now = DateTime.UtcNow;
                var activeOffer = menuItem.Offers.FirstOrDefault(o =>
                    o.IsActive &&
                    o.StartDate <= now &&
                    o.EndDate >= now
                );

                var basePrice = menuItemSize.Price;
                if (activeOffer != null)
                {
                    var discount = activeOffer.Price;
                    basePrice = Math.Max(0, basePrice - discount);
                }

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    Quantity = itemDto.Quantity,
                    Note = itemDto.Note,
                    SpicyLevel = itemDto.SpicyLevel,
                    Price = basePrice * itemDto.Quantity,
                    MenuItemId = menuItem.Id,
                    MenuItemSizeId = menuItemSize.Id,
                    OrderItemExtras = new List<OrderItemExtra>()
                };

                if (itemDto.ExtraIds != null && itemDto.ExtraIds.Count > 0)
                {
                    var extras = menuItem.Extras.Where(e => itemDto.ExtraIds.Contains(e.Id)).ToList();

                    foreach (var extra in extras)
                    {
                        orderItem.OrderItemExtras.Add(new OrderItemExtra
                        {
                            Id = Guid.NewGuid(),
                            ExtraId = extra.Id,
                            Price = extra.Price
                        });

                        orderItem.Price += extra.Price * itemDto.Quantity;
                    }
                }

                totalPrice += orderItem.Price;
                order.OrderItems.Add(orderItem);
            }

            if (orderType == OrderType.Delivery)
            {
                order.DeliveryFee = (decimal)(createOrderDto.DeliveryFee ?? 0);
                totalPrice += order.DeliveryFee;
            }

            order.Price = totalPrice;
            order.EstimatedPreparationTime = totalTime;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderDto = MapToOrderDto(order);
            orderDto.EstimatedPreparationTime = totalTime;

            await _hubContext.Clients.All.SendAsync("ReceiveNewOrder", orderDto);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
        }

        [HttpGet("GetAllOrders")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderDtos = orders.Select(o => MapToOrderDto(o)).ToList();
            return Ok(orderDtos);
        }

        [HttpGet("GetGroupedActiveOrders")]
        [Authorize]
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

        [HttpGet("OrderItemExtras/{orderItemId}")]
        public async Task<ActionResult<List<OrderItemExtraDto>>> GetOrderItemExtras(Guid orderItemId)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.OrderItemExtras)
                .ThenInclude(oiEx => oiEx.Extra)
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId);

            if (orderItem == null)
            {
                return NotFound(new { Message = "Order item not found" });
            }

            var extrasDto = orderItem.OrderItemExtras.Select(oiEx => new OrderItemExtraDto
            {
                Id = oiEx.Id,
                ExtraId = oiEx.ExtraId,
                Price = oiEx.Price,
                ImagePath = oiEx.Extra.ImagePath
            }).ToList();

            return Ok(extrasDto);
        }

        [HttpPut("UpdateStatus/{id}")]
        [Authorize(Roles = "admin,chef")]
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

            var orderLog = new OrderLog
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = status,
                UpdatedAt = DateTime.UtcNow
            };

            _context.OrderLogs.Add(orderLog);

            await _context.SaveChangesAsync();

            var orderDto = MapToOrderDto(order);

            await _hubContext.Clients.All.SendAsync("OrderStatusUpdated", orderDto);

            return Ok(orderDto);
        }

        [HttpGet("CustomerOrders/{customerId}")]
        [Authorize]
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
        [Authorize]
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

            return Ok(new
            {
                order.Id,
                order.OrderDate,
                order.EstimatedPreparationTime,
                ExpectedReadyTime = order.OrderDate + order.EstimatedPreparationTime
            });
        }

        [HttpGet("GetReadyDeliveryOrders")]
        [Authorize]
        public async Task<ActionResult<List<OrderDto>>> GetReadyDeliveryOrders()
        {
            var readyDeliveryOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Ready
                            && o.Type == OrderType.Delivery
                            && o.DeliveryId == null)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderDtos = readyDeliveryOrders.Select(o => MapToOrderDto(o)).ToList();

            return Ok(orderDtos);
        }

        [HttpGet("GetOrdersByDelivery/{deliveryId}")]
        [Authorize]
        public async Task<ActionResult<List<OrderDto>>> GetOrdersByDelivery(Guid deliveryId)
        {
            var orders = await _context.Orders
                .Where(o => o.DeliveryId == deliveryId)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderDtos = orders.Select(o => MapToOrderDto(o)).ToList();

            return Ok(orderDtos);
        }

        [HttpGet("GetReadyWaiterOrders")]
        [Authorize]
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