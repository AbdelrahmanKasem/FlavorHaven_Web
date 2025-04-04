using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;

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

        // GET: api/orders/{id}
        [HttpGet("GetOrder/{id}")]
        public IActionResult GetOrder(Guid id)
        {
            var order = _context.Orders
                .Where(o => o.Id == id)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    Type = o.Type,
                    Price = o.Price,
                    PaymentSystem = o.PaymentSystem,
                    Note = o.Note,
                    CustomerId = o.CustomerId,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        Quantity = oi.Quantity,
                        Note = oi.Note,
                        SpicyLevel = oi.SpicyLevel,
                        Price = oi.Price,
                        Customizations = oi.Customizations.Select(c => new OrderItemCustomizationDto
                        {
                            Id = c.Id,
                            Name = c.Name,
                            ExtraPrice = c.ExtraPrice
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            return Ok(order);
        }

        // POST: api/orders
        [HttpPost("CreateOrder")]
        public IActionResult CreateOrder([FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = new Order
            {
                Id = orderDto.Id,
                OrderDate = orderDto.OrderDate,
                Status = orderDto.Status,
                Type = orderDto.Type,
                Price = orderDto.Price,
                PaymentSystem = orderDto.PaymentSystem,
                Note = orderDto.Note,
                CustomerId = orderDto.CustomerId,
                OrderItems = orderDto.OrderItems.Select(oi => new OrderItem
                {
                    Id = oi.Id,
                    Quantity = oi.Quantity,
                    Note = oi.Note,
                    SpicyLevel = oi.SpicyLevel,
                    Price = oi.Price,
                    Customizations = oi.Customizations.Select(c => new OrderItemCustomization
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ExtraPrice = c.ExtraPrice
                    }).ToList()
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT: api/orders/{id}
        [HttpPut("UpdateOrder/{id}")]
        public IActionResult UpdateOrder(Guid id, [FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid || id != orderDto.Id)
            {
                return BadRequest();
            }

            var existingOrder = _context.Orders.Find(id);
            if (existingOrder == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            existingOrder.OrderDate = orderDto.OrderDate;
            existingOrder.Status = orderDto.Status;
            existingOrder.Type = orderDto.Type;
            existingOrder.Price = orderDto.Price;
            existingOrder.PaymentSystem = orderDto.PaymentSystem;
            existingOrder.Note = orderDto.Note;
            existingOrder.CustomerId = orderDto.CustomerId;

            // Update order items
            _context.OrderItems.RemoveRange(existingOrder.OrderItems);
            existingOrder.OrderItems = orderDto.OrderItems.Select(oi => new OrderItem
            {
                Id = oi.Id,
                Quantity = oi.Quantity,
                Note = oi.Note,
                SpicyLevel = oi.SpicyLevel,
                Price = oi.Price,
                Customizations = oi.Customizations.Select(c => new OrderItemCustomization
                {
                    Id = c.Id,
                    Name = c.Name,
                    ExtraPrice = c.ExtraPrice
                }).ToList()
            }).ToList();

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/orders/{id}
        [HttpDelete("DeleteOrder/{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/orders/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public IActionResult GetOrdersByCustomer(Guid customerId)
        {
            var orders = _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    Type = o.Type,
                    Price = o.Price,
                    PaymentSystem = o.PaymentSystem,
                    Note = o.Note,
                    CustomerId = o.CustomerId,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        Quantity = oi.Quantity,
                        Note = oi.Note,
                        SpicyLevel = oi.SpicyLevel,
                        Price = oi.Price,
                        Customizations = oi.Customizations.Select(c => new OrderItemCustomizationDto
                        {
                            Id = c.Id,
                            Name = c.Name,
                            ExtraPrice = c.ExtraPrice
                        }).ToList()
                    }).ToList()
                })
                .ToList();

            return Ok(orders);
        }
    }
}
