using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.DTOs;
using RMSProjectAPI.Model;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,cashier")]
    [ApiController]
    public class CashierController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CashierController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetPendingOrders")]
        public async Task<ActionResult<List<OrderDto>>> GetPendingOrders()
        {
            var orders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Pending)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderDtos = orders.Select(o => MapToOrderDto(o)).ToList();
            return Ok(orderDtos);
        }

        [HttpPost("MarkAsPaid/{orderId}")]
        public async Task<ActionResult> MarkAsPaid(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                return NotFound("Order not found");

            if (order.Status != OrderStatus.Pending)
                return BadRequest("Only pending orders can be marked as paid");

            order.Status = OrderStatus.Paid;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("GetOrder/{orderId}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound("Order not found");

            return Ok(MapToOrderDto(order));
        }

        [HttpGet("GetTodaySales")]
        public async Task<ActionResult<decimal>> GetTodaySales()
        {
            var today = DateTime.UtcNow.Date;

            var totalSales = await _context.Orders
                .Where(o => o.Status == OrderStatus.Paid && o.OrderDate.Date == today)
                .SumAsync(o => o.Price);

            return Ok(totalSales);
        }

        // Helper method (You can move this to a service if you want)
        private OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Type = order.Type,
                Latitude = order.Latitude,
                Longitude = order.Longitude,
                Address = order.Address,
                PaymentSystem = order.PaymentSystem,
                TransactionId = order.TransactionId,
                Note = order.Note,
                CustomerId = order.CustomerId,
                DeliveryId = order.DeliveryId,
                WaiterId = order.WaiterId,
                TableId = order.TableId,
                Price = order.Price,
                EstimatedPreparationTime = order.EstimatedPreparationTime,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    Quantity = oi.Quantity,
                    Note = oi.Note,
                    SpicyLevel = oi.SpicyLevel,
                    Price = oi.Price,
                    MenuItemId = oi.MenuItemId,
                    MenuItemSizeId = oi.MenuItemSizeId,
                    Extras = oi.OrderItemExtras?.Select(e => new OrderItemExtraDto
                    {
                        Id = e.ExtraId,
                        Price = e.Price
                    }).ToList()
                }).ToList()
            };
        }
    }
}