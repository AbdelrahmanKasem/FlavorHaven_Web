using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteMealController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoriteMealController(AppDbContext context)
        {
            _context = context;
        }

        // Get
        // GET: api/FavoriteMeal/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<FavoriteMealDto>>> GetUserFavoriteMeals(Guid userId)
        {
            var favorites = await _context.FavoriteMeals
                .Where(f => f.UserId == userId)
                .Select(f => new FavoriteMealDto
                {
                    Id = f.Id,
                    UserId = f.UserId,
                    MenuItemId = f.MenuItemId
                })
                .ToListAsync();

            return Ok(favorites);
        }

        // Add
        // POST: api/FavoriteMeal
        [HttpPost]
        public async Task<ActionResult> AddFavoriteMeal([FromBody] FavoriteMealDto favoriteMealDto)
        {
            if (await _context.FavoriteMeals.AnyAsync(f => f.UserId == favoriteMealDto.UserId && f.MenuItemId == favoriteMealDto.MenuItemId))
            {
                return BadRequest("This meal is already in the user's favorites.");
            }

            var favoriteMeal = new FavoriteMeal
            {
                Id = Guid.NewGuid(),
                UserId = favoriteMealDto.UserId,
                MenuItemId = favoriteMealDto.MenuItemId
            };

            _context.FavoriteMeals.Add(favoriteMeal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserFavoriteMeals), new { userId = favoriteMeal.UserId }, favoriteMealDto);
        }

        // Remove
        // DELETE: api/FavoriteMeal/{userId}/{menuItemId}
        [HttpDelete("{userId}/{menuItemId}")]
        public async Task<ActionResult> RemoveFavoriteMeal(Guid userId, Guid menuItemId)
        {
            var favoriteMeal = await _context.FavoriteMeals
                .FirstOrDefaultAsync(f => f.UserId == userId && f.MenuItemId == menuItemId);

            if (favoriteMeal == null)
            {
                return NotFound("Favorite meal not found.");
            }

            _context.FavoriteMeals.Remove(favoriteMeal);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
