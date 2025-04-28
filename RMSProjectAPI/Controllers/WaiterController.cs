using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaiterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WaiterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("AssignOrder/{orderId}")]
        [Authorize(Roles = "waiter")]
        public async Task<IActionResult> AssignOrder(Guid orderId)
        {
            var userId = User?.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Unauthorized access");

            var waiterId = Guid.Parse(userId);

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound("Order not found");

            if (order.WaiterId != null)
                return BadRequest("This order is already assigned to a waiter");

            order.WaiterId = waiterId;
            order.Status = OrderStatus.InProgress;

            await _context.SaveChangesAsync();

            return Ok("Order assigned to waiter successfully");
        }
    }
}
