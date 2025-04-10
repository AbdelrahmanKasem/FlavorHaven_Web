using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OfferController(AppDbContext context)
        {
            _context = context;
        }

        // Create a new offer
        [HttpPost("Create")]
        public async Task<ActionResult<OfferDto>> CreateOffer([FromBody] OfferDto offerDto)
        {
            var offer = new Offer
            {
                Id = Guid.NewGuid(),
                Title = offerDto.Title,
                Description = offerDto.Description,
                Price = offerDto.Price,
                ImagePath = offerDto.ImagePath,
                StartDate = offerDto.StartDate,
                EndDate = offerDto.EndDate,
                IsActive = offerDto.IsActive,
                MenuItemId = offerDto.MenuItemId
            };

            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            offerDto.Id = offer.Id;  // Set the created offer's Id in the DTO
            return Ok(offerDto);
        }

        // Delete an offer
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteOffer(Guid id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
                return NotFound("Offer not found.");

            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();

            return Ok("Offer deleted.");
        }

        // Update an existing offer
        [HttpPut("Update/{id}")]
        public async Task<ActionResult> UpdateOffer(Guid id, [FromBody] OfferDto offerDto)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
                return NotFound("Offer not found.");

            offer.Title = offerDto.Title;
            offer.Description = offerDto.Description;
            offer.Price = offerDto.Price;
            offer.ImagePath = offerDto.ImagePath;
            offer.StartDate = offerDto.StartDate;
            offer.EndDate = offerDto.EndDate;
            offer.IsActive = offerDto.IsActive;
            offer.MenuItemId = offerDto.MenuItemId;

            await _context.SaveChangesAsync();

            return Ok("Offer updated.");
        }

        // Get a specific offer by ID
        [HttpGet("GetOffer/{id}")]
        public async Task<ActionResult<OfferDto>> GetOffer(Guid id)
        {
            var offer = await _context.Offers
                .Include(o => o.MenuItem)  // Optionally, include the MenuItem data
                .FirstOrDefaultAsync(o => o.Id == id);

            if (offer == null)
                return NotFound("Offer not found.");

            var offerDto = new OfferDto
            {
                Id = offer.Id,
                Title = offer.Title,
                Description = offer.Description,
                Price = offer.Price,
                ImagePath = offer.ImagePath,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate,
                IsActive = offer.IsActive,
                MenuItemId = offer.MenuItemId
            };

            return Ok(offerDto);
        }

        // Get all offers
        [HttpGet("GetAllOffers")]
        public async Task<ActionResult<IEnumerable<OfferDto>>> GetAllOffers()
        {
            var offers = await _context.Offers
                .Include(o => o.MenuItem)
                .ToListAsync();

            var offerDtos = offers.Select(offer => new OfferDto
            {
                Id = offer.Id,
                Title = offer.Title,
                Description = offer.Description,
                Price = offer.Price,
                ImagePath = offer.ImagePath,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate,
                IsActive = offer.IsActive,
                MenuItemId = offer.MenuItemId
            }).ToList();

            return Ok(offerDtos);
        }

        // Get available offers based on start and end time
        [HttpGet("GetAvailableOffers")]
        public async Task<ActionResult<IEnumerable<OfferDto>>> GetAvailableOffers()
        {
            var currentDate = DateTime.Now;
            var offers = await _context.Offers
                .Where(o => o.StartDate <= currentDate && o.EndDate >= currentDate && o.IsActive)
                .Include(o => o.MenuItem)
                .ToListAsync();

            var offerDtos = offers.Select(offer => new OfferDto
            {
                Id = offer.Id,
                Title = offer.Title,
                Description = offer.Description,
                Price = offer.Price,
                ImagePath = offer.ImagePath,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate,
                IsActive = offer.IsActive,
                MenuItemId = offer.MenuItemId
            }).ToList();

            return Ok(offerDtos);
        }

        [HttpPost("AddOfferToCart")]
        public async Task<ActionResult<CartDto>> AddOfferToCart(Guid userId, Guid offerId)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

            if (cart == null)
            {
                cart = new Cart { Id = Guid.NewGuid(), UserId = userId };
                _context.Carts.Add(cart);
            }

            var offer = await _context.Offers
                .Include(o => o.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == offerId && o.IsActive);

            if (offer == null)
                return NotFound("Offer not found or inactive.");

            var currentDate = DateTime.Now;
            if (offer.StartDate > currentDate || offer.EndDate < currentDate)
                return BadRequest("Offer is not available at this time.");

            var cartItem = cart.Items.FirstOrDefault(i => i.MenuItemId == offer.MenuItemId && i.PriceAtTimeOfOrder == offer.Price);

            // Set quantity to 1 by default
            if (cartItem != null)
            {
                cartItem.Quantity += 1; // Always add 1 if the item is already in the cart
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    MenuItemId = offer.MenuItemId,
                    Quantity = 1, // Set quantity to 1 by default
                    PriceAtTimeOfOrder = offer.Price
                });
            }

            await _context.SaveChangesAsync();

            return Ok("Offer added to cart.");
        }

    }
}
