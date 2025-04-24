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

        // ✅ Get MenuItem by ID with related entities
        [HttpGet("GetMenuItem/{id}")]
        public async Task<IActionResult> GetMenuItemById(Guid id)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.Category)
                .Include(m => m.Extras)
                .Include(m => m.Sizes)
                .Include(m => m.Suggestions)
                .FirstOrDefaultAsync(m => m.Id == id);

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

            extraDto.Id = extra.Id;
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

            return NoContent();
        }

        // Update an Extra
        [HttpPut("UpdateExtra/{id}")]
        public async Task<ActionResult<ExtraDto>> UpdateExtra(Guid id, UpdateExtraDto extraDto)
        {
            var extra = await _context.Extras.FindAsync(id);
            if (extra == null)
                return NotFound("Extra not found.");

            extra.Name = extraDto.Name;
            extra.Price = extraDto.Price;
            extra.ImagePath = extraDto.ImagePath;
            extra.MenuItemId = extraDto.MenuItemId;

            await _context.SaveChangesAsync();

            return Ok(extraDto);
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
        public async Task<IActionResult> UpdateMenuItemSize(Guid id, [FromBody] UpdateMenuItemSizeDto sizeDto)
        {
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

        [HttpPost("{menuItemId}/suggestions")]
        public async Task<IActionResult> AddSuggestion(Guid menuItemId, [FromBody] AddSuggestionDto dto)
        {
            if (!await _context.MenuItems.AnyAsync(m => m.Id == menuItemId) ||
                !await _context.MenuItems.AnyAsync(m => m.Id == dto.SuggestedItemId))
                return NotFound("Menu item or suggested item not found");

            var exists = await _context.MenuItemSuggestions.AnyAsync(s =>
                s.MenuItemId == menuItemId && s.SuggestedItemId == dto.SuggestedItemId);

            if (exists)
                return BadRequest("This suggestion already exists");

            var suggestion = new MenuItemSuggestion
            {
                Id = Guid.NewGuid(),
                MenuItemId = menuItemId,
                SuggestedItemId = dto.SuggestedItemId
            };

            _context.MenuItemSuggestions.Add(suggestion);
            await _context.SaveChangesAsync();

            return Ok(suggestion);
        }

        [HttpDelete("{menuItemId}/suggestions/{suggestedItemId}")]
        public async Task<IActionResult> DeleteSuggestion(Guid menuItemId, Guid suggestedItemId)
        {
            var suggestion = await _context.MenuItemSuggestions.FirstOrDefaultAsync(s =>
                s.MenuItemId == menuItemId && s.SuggestedItemId == suggestedItemId);

            if (suggestion == null)
                return NotFound("Suggestion not found");

            _context.MenuItemSuggestions.Remove(suggestion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{menuItemId}/suggestions")]
        public async Task<IActionResult> GetSuggestions(Guid menuItemId)
        {
            var suggestions = await _context.MenuItemSuggestions
                .Where(s => s.MenuItemId == menuItemId)
                .Include(s => s.SuggestedItem)
                .Select(s => new SuggestedItemDto
                {
                    Id = s.SuggestedItem.Id,
                    Name = s.SuggestedItem.Name,
                    Price = s.SuggestedItem.Price,
                    ImageUrl = s.SuggestedItem.ImagePath,
                    RatingCount = s.SuggestedItem.RatingCount,
                    TotalRating = s.SuggestedItem.TotalRating,
                    Descritpion = s.SuggestedItem.Description
                }).ToListAsync();

            return Ok(suggestions);
        }

        // ✅ Search Menu Items
        [HttpGet("SearchMenuItems")]
        public async Task<IActionResult> SearchMenuItems([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query is required.");

            var results = await _context.MenuItems
                .Where(mi => mi.Name.Contains(query) || mi.Description.Contains(query))
                .Select(mi => new
                {
                    mi.Id,
                    mi.Name,
                    mi.Description,
                    mi.ImagePath,
                    mi.Price,
                    mi.Duration,
                    mi.RatingCount,
                    mi.TotalRating,
                })
                .ToListAsync();

            if (results.Count == 0)
                return NotFound("No menu items matched your search.");

            return Ok(results);
        }

        [HttpGet("TopSoldMenuItems")]
        public async Task<IActionResult> GetTopSoldMenuItemsLast30Days([FromQuery] int count = 5)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var topItems = await _context.OrderItems
                .Include(oi => oi.MenuItem)
                .Include(oi => oi.Order) // Needed to filter by OrderDate
                .Where(oi => oi.Order.OrderDate >= thirtyDaysAgo)
                .GroupBy(oi => new
                {
                    oi.MenuItemId,
                    oi.MenuItem.Name,
                    oi.MenuItem.ImagePath,
                    oi.MenuItem.Description
                })
                .Select(g => new
                {
                    MenuItemId = g.Key.MenuItemId,
                    Name = g.Key.Name,
                    ImagePath = g.Key.ImagePath,
                    Description = g.Key.Description,
                    TotalSold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(item => item.TotalSold)
                .Take(count)
                .ToListAsync();

            return Ok(topItems);
        }

        [HttpPost("Rate")]
        public async Task<IActionResult> RateMenuItem([FromBody] MenuItemRatingDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                return BadRequest("Rating must be between 1 and 5");

            var menuItem = await _context.MenuItems.FindAsync(dto.MenuItemId);
            if (menuItem == null)
                return NotFound("Menu item not found");

            // Update rating stats
            menuItem.TotalRating += dto.Rating;
            menuItem.RatingCount += 1;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                menuItem.Id,
                menuItem.Name,
                menuItem.TotalRating,
                menuItem.RatingCount,
                AverageRating = menuItem.RatingCount > 0
                    ? (double)menuItem.TotalRating / menuItem.RatingCount
                    : 0
            });
        }
    }
}
