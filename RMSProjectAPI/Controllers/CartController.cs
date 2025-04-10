//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using RMSProjectAPI.Database;
//using RMSProjectAPI.Database.Entity;
//using RMSProjectAPI.Model;

//namespace RMSProjectAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CartController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public CartController(AppDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("AddItem")]
//        public async Task<IActionResult> AddItemToCart(Guid userId, Guid menuItemId, Guid menuItemSizeId, int quantity)
//        {
//            // Get or create the user's cart
//            var cart = await _context.Carts
//                .Include(c => c.Items)
//                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

//            if (cart == null)
//            {
//                cart = new Cart
//                {
//                    Id = Guid.NewGuid(),
//                    UserId = userId,
//                    IsCheckedOut = false,
//                    TotalPrice = 0,
//                    Items = new List<CartItem>()
//                };
//                _context.Carts.Add(cart);
//            }

//            // Validate the selected MenuItem and Size
//            var menuItem = await _context.MenuItems
//                .Include(m => m.Sizes)
//                .FirstOrDefaultAsync(m => m.Id == menuItemId);

//            if (menuItem == null)
//                return NotFound("Menu item not found.");

//            var selectedSize = menuItem.Sizes.FirstOrDefault(s => s.Id == menuItemSizeId);
//            if (selectedSize == null)
//                return NotFound("Selected size not found.");

//            decimal price = selectedSize.Price;

//            // Create the cart item
//            var cartItem = new CartItem
//            {
//                Id = Guid.NewGuid(),
//                CartId = cart.Id,
//                MenuItemId = menuItemId,
//                MenuItemSizeId = menuItemSizeId,
//                MenuItemName = menuItem.Name,
//                MenuItemDescription = menuItem.Description,
//                MenuItemImage = menuItem.ImagePath,
//                PriceAtTimeOfOrder = price,
//                Quantity = quantity
//            };

//            cart.Items.Add(cartItem);
//            cart.TotalPrice = cart.Items.Sum(i => i.PriceAtTimeOfOrder * i.Quantity);

//            await _context.SaveChangesAsync();

//            return Ok(new
//            {
//                Message = "Item added to cart.",
//                CartId = cart.Id,
//                Total = cart.TotalPrice
//            });
//        }

//        [HttpDelete("DeleteItem")]
//        public async Task<IActionResult> DeleteItemFromCart(Guid userId, Guid cartItemId)
//        {
//            // Find the user's active cart
//            var cart = await _context.Carts
//                .Include(c => c.Items)
//                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

//            if (cart == null)
//            {
//                return NotFound("Cart not found for the user.");
//            }

//            // Find the item to delete
//            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
//            if (item == null)
//            {
//                return NotFound("Item not found in the cart.");
//            }

//            // Remove item and update total price
//            cart.Items.Remove(item);
//            _context.CartItems.Remove(item); // Explicit removal from DbContext
//            cart.TotalPrice = cart.Items.Sum(i => i.PriceAtTimeOfOrder * i.Quantity);

//            await _context.SaveChangesAsync();

//            return Ok(new
//            {
//                Message = "Item removed from cart.",
//                CartId = cart.Id,
//                Total = cart.TotalPrice
//            });
//        }

//    }
//}