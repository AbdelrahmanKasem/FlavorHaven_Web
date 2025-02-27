using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        // Create a new Menu
        [HttpPost("CreateMenu")]
        public async Task<IActionResult> CreateMenu([FromBody] MenuDto menuDto)
        {
            if (menuDto == null)
            {
                return BadRequest("Invalid menu data.");
            }

            var menu = new Menu
            {
                Id = Guid.NewGuid(),
                Offers = menuDto.Offers
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuById), new { id = menu.Id }, menu);
        }

        // Get all Menus
        [HttpGet("GetMenus")]
        public async Task<IActionResult> GetMenus()
        {
            var menus = await _context.Menus.ToListAsync();
            return Ok(menus);
        }

        // Get Menu by Id
        [HttpGet("GetMenu/{id}")]
        public async Task<IActionResult> GetMenuById(Guid id)
        {
            var menu = await _context.Menus.FindAsync(id);

            if (menu == null)
            {
                return NotFound($"Menu with ID {id} not found.");
            }

            return Ok(menu);
        }

        // Update Menu
        [HttpPut("UpdateMenu/{id}")]
        public async Task<IActionResult> UpdateMenu(Guid id, [FromBody] MenuDto menuDto)
        {
            if (menuDto == null)
            {
                return BadRequest("Invalid menu data.");
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound($"Menu with ID {id} not found.");
            }

            menu.Offers = menuDto.Offers;

            _context.Menus.Update(menu);
            await _context.SaveChangesAsync();

            return Ok(menu);
        }

        // Delete Menu
        [HttpDelete("DeleteMenu/{id}")]
        public async Task<IActionResult> DeleteMenu(Guid id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound($"Menu with ID {id} not found.");
            }

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Create a new Category
        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null || string.IsNullOrWhiteSpace(categoryDto.Name))
            {
                return BadRequest("Category name is required.");
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categoryDto.Name,
                Offers = categoryDto.Offers
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        // ✅ Get Category By Id
        [HttpGet("GetCategory/{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // ✅ Update Category
        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null || string.IsNullOrWhiteSpace(categoryDto.Name))
            {
                return BadRequest("Category name is required.");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            category.Name = categoryDto.Name;
            category.Offers = categoryDto.Offers;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // ✅ Delete Category
        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 status code, indicating successful deletion with no content
        }

        // ✅ Create new menu item
        [HttpPost("CreateMenuItem")]
        public async Task<IActionResult> CreateMenuItem([FromBody] MenuItemDto model)
        {
            if (model == null)
            {
                return BadRequest("Invalid menu item data.");
            }

            // Check if the category exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == model.CategoryId);
            if (!categoryExists)
            {
                return NotFound($"Category with ID {model.CategoryId} not found.");
            }

            var newItem = new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                ImagePath = model.ImagePath,
                Price = model.Price,
                Duration = model.Duration,
                Offers = model.Offers,
                CategoryId = model.CategoryId
            };

            _context.MenuItems.Add(newItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenuItemById), new { id = newItem.Id }, newItem);
        }

        // ✅ Get Menu Item By Id
        [HttpGet("GetMenuItem/{id}")]
        public async Task<IActionResult> GetMenuItemById(Guid id)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.Category) // Include category details
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
            {
                return NotFound($"Menu item with ID {id} not found.");
            }

            return Ok(menuItem);
        }

        // ✅ Update Menu Item
        [HttpPut("UpdateMenuItem/{id}")]
        public async Task<IActionResult> UpdateMenuItem(Guid id, [FromBody] MenuItemDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest("Invalid menu item data.");
            }

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound($"Menu item with ID {id} not found.");
            }

            // Check if the category exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == model.CategoryId);
            if (!categoryExists)
            {
                return NotFound($"Category with ID {model.CategoryId} not found.");
            }

            menuItem.Name = model.Name;
            menuItem.Description = model.Description;
            menuItem.ImagePath = model.ImagePath;
            menuItem.Price = model.Price;
            menuItem.Duration = model.Duration;
            menuItem.Offers = model.Offers;
            menuItem.CategoryId = model.CategoryId;

            _context.MenuItems.Update(menuItem);
            await _context.SaveChangesAsync();

            return Ok(menuItem);
        }

        // ✅ Delete Menu Item
        [HttpDelete("DeleteMenuItem/{id}")]
        public async Task<IActionResult> DeleteMenuItem(Guid id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound($"Menu item with ID {id} not found.");
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 status code, indicating successful deletion with no content
        }


        // ✅ Get all menu items
        [HttpGet("GetMenuItems")]
        public async Task<IActionResult> GetMenuItems()
        {
            var menuItems = await _context.MenuItems
                .Include(m => m.Category) // Include Category details
                .ToListAsync();
            return Ok(menuItems);
        }

        //// Get menu items by category
        //[HttpGet("GetItemsByCategory/{categoryId}")]
        //public async Task<IActionResult> GetItemsByCategory(Guid categoryId)
        //{
        //    var items = await _context.MenuItems
        //        .Where(m => m.CategoryId == categoryId)
        //        .Include(m => m.Category) // Include Category details
        //        .ToListAsync();

        //    if (items == null || !items.Any())
        //    {
        //        return NotFound($"No menu items found for Category ID: {categoryId}");
        //    }

        //    return Ok(items);
        //}

        //// Search in menu items
        //[HttpGet("SearchMenuItems")]
        //public async Task<IActionResult> SearchMenuItems(
        //    [FromQuery] string? name,
        //    [FromQuery] Guid? categoryId,
        //    [FromQuery] decimal? minPrice,
        //    [FromQuery] decimal? maxPrice,
        //    [FromQuery] bool? hasOffer)
        //{
        //    var query = _context.MenuItems.AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(name))
        //    {
        //        query = query.Where(m => m.Name.Contains(name));
        //    }

        //    if (categoryId.HasValue)
        //    {
        //        query = query.Where(m => m.CategoryId == categoryId);
        //    }

        //    if (minPrice.HasValue)
        //    {
        //        query = query.Where(m => m.Price >= minPrice);
        //    }

        //    if (maxPrice.HasValue)
        //    {
        //        query = query.Where(m => m.Price <= maxPrice);
        //    }

        //    if (hasOffer.HasValue)
        //    {
        //        if (hasOffer.Value)
        //        {
        //            query = query.Where(m => m.Offers != null && m.Offers > 0);
        //        }
        //        else
        //        {
        //            query = query.Where(m => m.Offers == null || m.Offers == 0);
        //        }
        //    }

        //    var results = await query.ToListAsync();

        //    return Ok(results);
        //}
    }
}
