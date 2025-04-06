using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using RMSProjectAPI.Database;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Create Menu
        [HttpPost("CreateMenu")]
        public async Task<IActionResult> CreateMenu([FromBody] MenuDto menuDto)
        {
            if (menuDto == null)
                return BadRequest("Invalid menu data.");

            var menu = new Menu
            {
                Id = Guid.NewGuid(),
                Offers = menuDto.Offers,
            };

            await _context.Menus.AddAsync(menu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuById), new { id = menu.Id }, menu);
        }

        // ✅ Get all Menus
        [HttpGet("GetMenus")]
        public async Task<IActionResult> GetAllMenus()
        {
            var menus = await _context.Menus.ToListAsync();
            return Ok(menus);
        }

        // ✅ Get Menu by ID
        [HttpGet("GetMenu/{id}")]
        public async Task<IActionResult> GetMenuById(Guid id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return NotFound();
            return Ok(menu);
        }

        // ✅ Update Menu
        [HttpPut("UpdateMenu/{id}")]
        public async Task<IActionResult> UpdateMenu(Guid id, [FromBody] MenuDto menuDto)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return NotFound();

            menu.Offers = menuDto.Offers;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Delete Menu
        [HttpDelete("DeleteMenu/{id}")]
        public async Task<IActionResult> DeleteMenu(Guid id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return NotFound();

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Create Category
        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
                return BadRequest("Invalid category data.");

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categoryDto.Name,
                MenuId = categoryDto.MenuId
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        // ✅ Get all Categories
        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        // ✅ Get all Categories by Menu ID
        [HttpGet("GetCategoriesByMenu/{menuId}")]
        public async Task<IActionResult> GetCategoriesByMenuId(Guid menuId)
        {
            var categories = await _context.Categories.Where(c => c.MenuId == menuId).ToListAsync();
            return Ok(categories);
        }

        // ✅ Get Category by ID
        [HttpGet("GetCategory/{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // ✅ Update Category
        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            category.Name = categoryDto.Name;
            category.MenuId = categoryDto.MenuId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Delete Category
        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Create MenuItem
        [HttpPost("CreateMenuItem")]
        public async Task<IActionResult> CreateMenuItem([FromBody] MenuItemDto menuItemDto)
        {
            if (menuItemDto == null)
                return BadRequest("Invalid menu item data.");

            var menuItem = new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = menuItemDto.Name,
                Description = menuItemDto.Description,
                ImagePath = menuItemDto.ImagePath,
                Price = menuItemDto.Price,
                Duration = menuItemDto.Duration,
                Offers = menuItemDto.Offers,
                CategoryId = menuItemDto.CategoryId
            };

            await _context.MenuItems.AddAsync(menuItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuItemById), new { id = menuItem.Id }, menuItem);
        }

        // ✅ Get all MenuItems
        [HttpGet("GetMenuItems")]
        public async Task<IActionResult> GetAllMenuItems()
        {
            var menuItems = await _context.MenuItems.ToListAsync();
            return Ok(menuItems);
        }

        // ✅ Get MenuItems by Category ID
        [HttpGet("GetMenuItemsByCategory/{categoryId}")]
        public async Task<IActionResult> GetMenuItemsByCategoryId(Guid categoryId)
        {
            var menuItems = await _context.MenuItems.Where(m => m.CategoryId == categoryId).ToListAsync();
            return Ok(menuItems);
        }

        // ✅ Get MenuItems by Menu ID
        [HttpGet("GetMenuItemsByMenu/{menuId}")]
        public async Task<IActionResult> GetMenuItemsByMenuId(Guid menuId)
        {
            var menuItems = await _context.MenuItems
                .Where(mi => _context.Categories.Any(c => c.Id == mi.CategoryId && c.MenuId == menuId))
                .ToListAsync();

            return Ok(menuItems);
        }

        // ✅ Get MenuItem by ID
        [HttpGet("GetMenuItem/{id}")]
        public async Task<IActionResult> GetMenuItemById(Guid id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound();
            return Ok(menuItem);
        }

        // ✅ Update MenuItem
        [HttpPut("UpdateMenuItem/{id}")]
        public async Task<IActionResult> UpdateMenuItem(Guid id, [FromBody] MenuItemDto menuItemDto)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound();

            menuItem.Name = menuItemDto.Name;
            menuItem.Description = menuItemDto.Description;
            menuItem.ImagePath = menuItemDto.ImagePath;
            menuItem.Price = menuItemDto.Price;
            menuItem.Duration = menuItemDto.Duration;
            menuItem.Offers = menuItemDto.Offers;
            menuItem.CategoryId = menuItemDto.CategoryId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Delete MenuItem
        [HttpDelete("DeleteMenuItem/{id}")]
        public async Task<IActionResult> DeleteMenuItem(Guid id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound();

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Search in menu items
        [HttpGet("SearchMenuItems")]
        public async Task<IActionResult> SearchMenuItems(
            [FromQuery] string? name,
            [FromQuery] Guid? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? hasOffer)
        {
            var query = _context.MenuItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(m => m.Name.Contains(name));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(m => m.CategoryId == categoryId);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(m => m.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(m => m.Price <= maxPrice);
            }

            if (hasOffer.HasValue)
            {
                if (hasOffer.Value)
                {
                    query = query.Where(m => m.Offers != null && m.Offers > 0);
                }
                else
                {
                    query = query.Where(m => m.Offers == null || m.Offers == 0);
                }
            }

            var results = await query.ToListAsync();

            return Ok(results);
        }

        // ✅ Get Top 10 Sold Dishes
        [HttpGet("TopSoldDishes")]
        public async Task<IActionResult> GetTopSoldDishes()
        {
            var topDishes = await _context.MenuItems
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.ImagePath,
                    m.Price,
                    TotalSold = _context.OrderItems.Where(oi => oi.Order.Status == OrderStatus.Completed && oi.Order.Status != OrderStatus.Cancelled && oi.OrderId != Guid.Empty)
                                                   .Where(oi => oi.OrderId != Guid.Empty && oi.Customizations.Count == 0)
                                                   .Sum(oi => oi.Quantity)
                })
                .OrderByDescending(m => m.TotalSold)
                .Take(10)
                .ToListAsync();

            return Ok(topDishes);
        }

    }
}
