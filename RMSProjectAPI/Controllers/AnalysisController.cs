using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize (Roles = "admin")]
    public class AnalysisController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalysisController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("TotalRevenue")]
        public async Task<IActionResult> GetTotalRevenue(DateTime startDate, DateTime endDate)
        {
            var totalRevenue = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == OrderStatus.Completed)
                .SumAsync(o => o.Price + o.DeliveryFee);  // Summing both order price and delivery fee

            return Ok(new { TotalRevenue = totalRevenue });
        }

        [HttpGet("OrderStats")]
        public async Task<IActionResult> GetOrderStats(DateTime startDate, DateTime endDate)
        {
            var totalOrders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .CountAsync();

            var completedOrders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == OrderStatus.Completed)
                .CountAsync();

            var cancelledOrders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == OrderStatus.Cancelled)
                .CountAsync();

            return Ok(new
            {
                TotalOrders = totalOrders,
                CompletedOrders = completedOrders,
                CancelledOrders = cancelledOrders
            });
        }

        [HttpGet("MostPopularMenuItems")]
        public async Task<IActionResult> GetMostPopularMenuItems(DateTime startDate, DateTime endDate)
        {
            var popularItems = await _context.OrderItems
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .GroupBy(oi => oi.MenuItemId)
                .Select(g => new
                {
                    MenuItemId = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(oi => oi.Price)
                })
                .OrderByDescending(x => x.TotalOrders)
                .ToListAsync();

            return Ok(popularItems);
        }

        [HttpGet("DeliveryStats")]
        public async Task<IActionResult> GetDeliveryStats(DateTime startDate, DateTime endDate)
        {
            var totalDeliveries = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Type == OrderType.Delivery)
                .CountAsync();

            var totalDeliveryRevenue = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Type == OrderType.Delivery)
                .SumAsync(o => o.DeliveryFee);

            return Ok(new
            {
                TotalDeliveries = totalDeliveries,
                TotalDeliveryRevenue = totalDeliveryRevenue
            });
        }

        [HttpGet("CustomerStats")]
        public async Task<IActionResult> GetCustomerStats(DateTime startDate, DateTime endDate)
        {
            var totalCustomers = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Select(o => o.CustomerId)
                .Distinct()
                .CountAsync();

            var totalCustomerRevenue = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .SumAsync(o => o.Price);

            return Ok(new
            {
                TotalCustomers = totalCustomers,
                TotalCustomerRevenue = totalCustomerRevenue
            });
        }

        [HttpGet("EmployeeStats")]
        public async Task<IActionResult> GetEmployeeStats(DateTime startDate, DateTime endDate)
        {
            var waiterStats = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.WaiterId != null)
                .GroupBy(o => o.WaiterId)
                .Select(g => new
                {
                    WaiterId = g.Key,
                    TotalOrdersHandled = g.Count(),
                    TotalRevenue = g.Sum(o => o.DeliveryFee)
                })
                .ToListAsync();

            var deliveryStats = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.DeliveryId != null)
                .GroupBy(o => o.DeliveryId)
                .Select(g => new
                {
                    DeliveryId = g.Key,
                    TotalOrdersHandled = g.Count(),
                    TotalRevenue = g.Sum(o => + o.DeliveryFee)
                })
                .ToListAsync();

            return Ok(new
            {
                WaiterStats = waiterStats,
                DeliveryStats = deliveryStats
            });
        }

        [HttpGet("RevenueByOrderType")]
        public async Task<IActionResult> GetRevenueByOrderType(DateTime startDate, DateTime endDate)
        {
            var revenueByType = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.Type)
                .Select(g => new
                {
                    OrderType = g.Key.ToString(),
                    TotalRevenue = g.Sum(o => o.Price)
                })
                .ToListAsync();

            return Ok(revenueByType);
        }
    }
}
