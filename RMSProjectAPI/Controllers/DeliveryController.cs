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
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DeliveryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Details/{deliveryId}")]
        public async Task<IActionResult> GetDeliveryPersonDetails(Guid deliveryId)
        {
            var user = await _context.Users
                .Where(u => u.Id == deliveryId)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("Delivery person not found");

            var orders = await _context.Orders
                .Where(o => o.DeliveryId == deliveryId)
                .ToListAsync();

            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            var completedOrders = orders.Where(o => o.Status == OrderStatus.Completed).ToList();
            var cancelledOrders = orders.Where(o => o.Status == OrderStatus.Cancelled).ToList();

            var monthlyRevenue = completedOrders
                .Where(o => o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear)
                .Sum(o => o.DeliveryFee);

            var response = new DeliveryDto
            {
                FullName = $"{user.FirstName} {user.LastName}",
                ImagePath = user.ImagePath,
                PhoneNumber = user.PhoneNumber ?? "Not Available",
                MonthlyRevenue = monthlyRevenue,
                CompletedOrders = completedOrders.Count,
                CancelledOrders = cancelledOrders.Count
            };

            return Ok(response);
        }

        [HttpPost("AssignOrder/{orderId}")]
        [Authorize(Roles = "delivery")]
        public async Task<IActionResult> AssignOrder(Guid orderId)
        {
            var userId = User?.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Unauthorized access");

            var deliveryId = Guid.Parse(userId);

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound("Order not found");

            if (order.Type != OrderType.Delivery)
                return BadRequest("Order is not for delivery");

            if (order.DeliveryId != null)
                return BadRequest("This order is already assigned to another delivery person");

            order.DeliveryId = deliveryId;
            order.Status = OrderStatus.InProgress;

            await _context.SaveChangesAsync();

            return Ok("Order assigned successfully");
        }

        //[HttpGet("OrderHistory/{deliveryId}")]
        //public async Task<IActionResult> GetOrderHistory(Guid deliveryId)
        //{
        //    var orders = await _context.Orders
        //        .Where(o => o.DeliveryId == deliveryId)
        //        .Include(o => o.Customer)
        //        .ToListAsync();

        //    if (!orders.Any())
        //        return NotFound("No orders found for this delivery person");

        //    var orderHistory = orders.Select(o => new OrderHistoryDto
        //    {
        //        OrderId = o.Id,
        //        OrderDate = o.OrderDate,
        //        Status = o.Status,
        //        DeliveryFee = o.DeliveryFee,
        //        Address = o.Address ?? "Not Provided"
        //    }).ToList();

        //    return Ok(orderHistory);
        //}

        [HttpGet("OrderHistory/{deliveryId}")]
        public async Task<IActionResult> GetOrderHistory(Guid deliveryId)
        {
            var orders = await _context.Orders
                .Where(o => o.DeliveryId == deliveryId)
                .Include(o => o.Customer)
                .ToListAsync();

            if (!orders.Any())
                return NotFound("No orders found for this delivery person");

            var orderHistory = orders.Select(o => new OrderHistoryDto
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                DeliveryFee = o.DeliveryFee,
                Address = o.Address ?? "Not Provided",

                CustomerId = o.CustomerId,
                CustomerName = $"{o.Customer.FirstName} {o.Customer.LastName}",
                CustomerImage = o.Customer.ImagePath ?? "DefaultImagePath"
            }).ToList();

            return Ok(orderHistory);
        }

    }
}