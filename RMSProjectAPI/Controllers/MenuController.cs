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

        // ✅ Get all Categories with their Menu Items by Menu ID
        [HttpGet("GetAllMenuItems/{menuId}")]
        public async Task<IActionResult> GetCategoriesWithMenuItems(Guid menuId)
        {
            var categories = await _context.Categories
                .Where(c => c.MenuId == menuId)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    Items = _context.MenuItems
                        .Where(mi => mi.CategoryId == c.Id)
                        .Select(mi => new
                        {
                            mi.Id,
                            mi.Name,
                            mi.Description,
                            mi.ImagePath,
                            mi.Price
                        })
                        .ToList()
                })
                .ToListAsync();

            if (categories == null || categories.Count == 0)
                return NotFound("No categories or menu items found for this menu.");

            return Ok(categories);
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
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto categoryDto)
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
                //Duration = menuItemDto.Duration,
                //Offers = menuItemDto.Offers,
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

        [HttpGet("GetMenuItemsByCategory/{categoryId}")]
        public async Task<IActionResult> GetMenuItemsByCategoryId(Guid categoryId, [FromQuery] Guid? userId = null)
        {
            var menuItemsQuery = _context.MenuItems
                .Where(m => m.CategoryId == categoryId)
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Description,
                    m.ImagePath,
                    m.Price,
                    m.Duration,
                    m.TotalRating,
                    m.RatingCount,
                    m.CategoryId,
                    AverageRating = m.RatingCount > 0 ? (double)m.TotalRating / m.RatingCount : 0,
                    IsFavorite = userId != null && _context.FavoriteMeals.Any(f => f.MenuItemId == m.Id && f.UserId == userId)
                });

            var menuItems = await menuItemsQuery.ToListAsync();

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
        public async Task<IActionResult> UpdateMenuItem(Guid id, [FromBody] UpdateMenuItemDto menuItemDto)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
                return NotFound();

            menuItem.Name = menuItemDto.Name;
            menuItem.Description = menuItemDto.Description;
            menuItem.ImagePath = menuItemDto.ImagePath;
            menuItem.Price = menuItemDto.Price;
            menuItem.Duration = menuItemDto.Duration;

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

        // ========================== Extra ================================
        // Create an Extra
        [HttpPost("CreateExtra")]
        public async Task<ActionResult<ExtraDto>> CreateExtra(ExtraDto extraDto)
        {
            if (extraDto == null)
                return BadRequest("Invalid extra data.");

            var menuItemExists = await _context.MenuItems.AnyAsync(m => m.Id == extraDto.MenuItemId);
            if (!menuItemExists)
                return NotFound("Menu item not found.");

            var extra = new Extra
            {
                Id = Guid.NewGuid(),
                Name = extraDto.Name,
                Price = extraDto.Price,
                ImagePath = extraDto.ImagePath,
                MenuItemId = extraDto.MenuItemId
            };

            _context.Extras.Add(extra);
            await _context.SaveChangesAsync();

            extraDto.Id = extra.Id; // Return the created Extra with Id
            return CreatedAtAction(nameof(GetExtra), new { id = extraDto.Id }, extraDto);
        }

        // Delete an Extra
        [HttpDelete("DeleteExtra/{id}")]
        public async Task<ActionResult> DeleteExtra(Guid id)
        {
            var extra = await _context.Extras.FindAsync(id);

            if (extra == null)
                return NotFound("Extra not found.");

            _context.Extras.Remove(extra);
            await _context.SaveChangesAsync();

            return NoContent(); // Successfully deleted
        }

        // Update an Extra
        [HttpPut("UpdateExtra/{id}")]
        public async Task<ActionResult<ExtraDto>> UpdateExtra(Guid id, ExtraDto extraDto)
        {
            if (id != extraDto.Id)
                return BadRequest("ID mismatch.");

            var extra = await _context.Extras.FindAsync(id);
            if (extra == null)
                return NotFound("Extra not found.");

            extra.Name = extraDto.Name;
            extra.Price = extraDto.Price;
            extra.ImagePath = extraDto.ImagePath;
            extra.MenuItemId = extraDto.MenuItemId;

            await _context.SaveChangesAsync();

            return Ok(extraDto); // Return updated ExtraDto
        }

        // Get all Extras
        [HttpGet("GetExtras")]
        public async Task<ActionResult<IEnumerable<ExtraDto>>> GetExtras()
        {
            var extras = await _context.Extras
                .Include(e => e.MenuItem)
                .Select(e => new ExtraDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Price = e.Price,
                    ImagePath = e.ImagePath,
                    MenuItemId = e.MenuItemId
                })
                .ToListAsync();

            return Ok(extras);
        }

        // Get a specific Extra by ID
        [HttpGet("GetExtra/{id}")]
        public async Task<ActionResult<ExtraDto>> GetExtra(Guid id)
        {
            var extra = await _context.Extras
                .Include(e => e.MenuItem)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (extra == null)
                return NotFound("Extra not found.");

            var extraDto = new ExtraDto
            {
                Id = extra.Id,
                Name = extra.Name,
                Price = extra.Price,
                ImagePath = extra.ImagePath,
                MenuItemId = extra.MenuItemId
            };

            return Ok(extraDto);
        }

        // Get Extras for a specific MenuItem
        [HttpGet("GetExtrasOfMenuItem/{menuItemId}")]
        public async Task<ActionResult<IEnumerable<ExtraDto>>> GetExtrasForMenuItem(Guid menuItemId)
        {
            var menuItemExists = await _context.MenuItems.AnyAsync(m => m.Id == menuItemId);
            if (!menuItemExists)
                return NotFound("Menu item not found.");

            var extras = await _context.Extras
                .Where(e => e.MenuItemId == menuItemId)
                .Select(e => new ExtraDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Price = e.Price,
                    ImagePath = e.ImagePath,
                    MenuItemId = e.MenuItemId
                })
                .ToListAsync();

            return Ok(extras);
        }

        // =========================== Sizes ===========================================
        [HttpPost("CreateMenuItemSize")]
        public async Task<IActionResult> CreateMenuItemSize([FromBody] MenuItemSizeDto sizeDto)
        {
            if (sizeDto == null)
                return BadRequest("Invalid size data.");

            var menuItemExists = await _context.MenuItems.AnyAsync(m => m.Id == sizeDto.MenuItemId);
            if (!menuItemExists)
                return NotFound("Menu item not found.");

            var menuItemSize = new MenuItemSize
            {
                Id = Guid.NewGuid(),
                Grams = sizeDto.Grams,
                Price = sizeDto.Price,
                MenuItemId = sizeDto.MenuItemId
            };

            _context.MenuItemSizes.Add(menuItemSize);
            await _context.SaveChangesAsync();

            sizeDto.Id = menuItemSize.Id;
            return CreatedAtAction(nameof(GetMenuItemSizeById), new { id = sizeDto.Id }, sizeDto);
        }

        // Get all sizes for a specific menu item
        [HttpGet("GetMenuItemSizes/{menuItemId}")]
        public async Task<IActionResult> GetMenuItemSizesByMenuItemId(Guid menuItemId)
        {
            var sizes = await _context.MenuItemSizes
                .Where(s => s.MenuItemId == menuItemId)
                .ToListAsync();

            return Ok(sizes);
        }

        // Get a specific size by its ID
        [HttpGet("GetMenuItemSize/{id}")]
        public async Task<IActionResult> GetMenuItemSizeById(Guid id)
        {
            var size = await _context.MenuItemSizes.FindAsync(id);
            if (size == null)
                return NotFound("Size not found.");

            return Ok(size);
        }

        // Update a size
        [HttpPut("UpdateMenuItemSize/{id}")]
        public async Task<IActionResult> UpdateMenuItemSize(Guid id, [FromBody] MenuItemSizeDto sizeDto)
        {
            if (id != sizeDto.Id)
                return BadRequest("ID mismatch.");

            var size = await _context.MenuItemSizes.FindAsync(id);
            if (size == null)
                return NotFound("Size not found.");

            size.Grams = sizeDto.Grams;
            size.Price = sizeDto.Price;
            size.MenuItemId = sizeDto.MenuItemId;

            await _context.SaveChangesAsync();

            return Ok(sizeDto);
        }

        // Delete a size
        [HttpDelete("DeleteMenuItemSize/{id}")]
        public async Task<IActionResult> DeleteMenuItemSize(Guid id)
        {
            var size = await _context.MenuItemSizes.FindAsync(id);
            if (size == null)
                return NotFound("Size not found.");

            _context.MenuItemSizes.Remove(size);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
