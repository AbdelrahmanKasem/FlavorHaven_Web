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

        // ✅ Tested
        [HttpPost("CreateOffer")]
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

            offerDto.Id = offer.Id;
            return Ok(offerDto);
        }

        // ✅ Tested
        [HttpDelete("DeleteOffer/{id}")]
        public async Task<ActionResult> DeleteOffer(Guid id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
                return NotFound("Offer not found.");

            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();

            return Ok("Offer deleted.");
        }

        // ✅ Tested
        [HttpPut("UpdateOffer/{id}")]
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

        // ✅ Tested
        [HttpGet("GetOfferOffer/{id}")]
        public async Task<ActionResult<OfferDto>> GetOffer(Guid id)
        {
            var offer = await _context.Offers
                .Include(o => o.MenuItem)
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

        // ✅ Tested
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

        // ✅ Tested
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
    }
}
