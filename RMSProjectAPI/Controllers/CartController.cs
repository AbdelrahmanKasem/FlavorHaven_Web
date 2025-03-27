using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ActionResult<CartDto>> GetCart(Guid userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.MenuItem) // Ensure MenuItem is loaded
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

            if (cart == null)
                return NotFound("Cart not found.");

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                IsCheckedOut = cart.IsCheckedOut,
                TotalPrice = cart.TotalPrice,
                Items = cart.Items
                    .Where(i => i.MenuItem != null) // Avoid null references
                    .Select(i => new MenuItemDto
                    {
                        Id = i.MenuItemId,
                        Name = i.MenuItem?.Name ?? "Unknown Item",
                        Price = i.MenuItem?.Price ?? 0,
                        Quantity = i.Quantity
                    }).ToList()
            };
        }

        [HttpPost("AddToCart")]
        public async Task<ActionResult<CartDto>> AddToCart(Guid userId, Guid menuItemId, int quantity)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

            if (cart == null)
            {
                cart = new Cart { Id = Guid.NewGuid(), UserId = userId };
                _context.Carts.Add(cart);
            }

            var menuItem = await _context.MenuItems.FindAsync(menuItemId);
            if (menuItem == null)
                return NotFound("Menu item not found.");

            var cartItem = cart.Items.FirstOrDefault(i => i.MenuItemId == menuItemId);
            if (cartItem != null)
                cartItem.Quantity += quantity;
            else
                cart.Items.Add(new CartItem { Id = Guid.NewGuid(), CartId = cart.Id, MenuItemId = menuItemId, Quantity = quantity });

            await _context.SaveChangesAsync();
            return Ok("Item added to cart.");
        }

        [HttpPut("Checkout/{userId}")]
        public async Task<ActionResult> Checkout(Guid userId)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

            if (cart == null || !cart.Items.Any())
                return BadRequest("Cart is empty or does not exist.");

            cart.IsCheckedOut = true;
            await _context.SaveChangesAsync();
            return Ok("Checkout successful.");
        }

        [HttpDelete("ClearCart{userId}")]
        public async Task<ActionResult> ClearCart(Guid userId)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

            if (cart == null)
                return NotFound("Cart not found.");

            // Clear the items from the cart
            cart.Items.Clear();

            await _context.SaveChangesAsync();
            return Ok("Cart cleared successfully.");
        }

        [HttpDelete("DeleteCartItem{userId}/item/{menuItemId}")]
        public async Task<ActionResult> RemoveCartItem(Guid userId, Guid menuItemId)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

            if (cart == null)
                return NotFound("Cart not found.");

            var cartItem = cart.Items.FirstOrDefault(i => i.MenuItemId == menuItemId);
            if (cartItem == null)
                return NotFound("Item not found in cart.");

            cart.Items.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok("Item removed from cart.");
        }
    }
}