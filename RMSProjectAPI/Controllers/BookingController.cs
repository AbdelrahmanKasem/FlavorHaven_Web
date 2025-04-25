using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetBookings")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            return await _context.Bookings
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    Date = b.Date,
                    Time = b.Time,
                    Duration = b.Duration,
                    Status = b.Status,
                    GuestCount = b.GuestCount,
                    TransactionId = b.TransactionId,
                    CustomerId = b.CustomerId,
                    TableId = b.TableId
                }).ToListAsync();
        }

        [HttpGet("GetBooking/{id}")]
        [Authorize]
        public async Task<ActionResult<BookingDto>> GetBooking(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null) return NotFound();

            return new BookingDto
            {
                Id = booking.Id,
                Date = booking.Date,
                Time = booking.Time,
                Duration = booking.Duration,
                Status = booking.Status,
                GuestCount = booking.GuestCount,
                TransactionId = booking.TransactionId,
                CustomerId = booking.CustomerId,
                TableId = booking.TableId
            };
        }

        [HttpPost("CreateBooking")]
        [Authorize]
        public async Task<ActionResult<BookingDto>> SmartCreateBooking(CreateBookingDto dto)
        {
            DateTime bookingStart = dto.Date.Date + dto.Time;
            DateTime bookingEnd = bookingStart + dto.Duration;

            var suitableTables = await _context.Tables
                .Where(t => t.Capacity >= dto.GuestCount)
                .OrderBy(t => t.Capacity)
                .Include(t => t.Bookings)
                .ToListAsync();

            foreach (var table in suitableTables)
            {
                bool isAvailable = table.Bookings.All(b =>
                {
                    var existingStart = b.Date.Date + b.Time;
                    var existingEnd = existingStart + b.Duration;
                    return bookingEnd <= existingStart || bookingStart >= existingEnd;
                });

                if (isAvailable)
                {
                    var newBooking = new Booking
                    {
                        Id = Guid.NewGuid(),
                        Date = dto.Date,
                        Time = dto.Time,
                        Duration = dto.Duration,
                        GuestCount = dto.GuestCount,
                        Status = BookingStatus.Pending,
                        TransactionId = dto.TransactionId,
                        CustomerId = dto.CustomerId,
                        TableId = table.Id
                    };

                    _context.Bookings.Add(newBooking);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetBooking), new { id = newBooking.Id }, new BookingDto
                    {
                        Id = newBooking.Id,
                        Date = newBooking.Date,
                        Time = newBooking.Time,
                        Duration = newBooking.Duration,
                        Status = newBooking.Status,
                        GuestCount = newBooking.GuestCount,
                        TransactionId = newBooking.TransactionId,
                        CustomerId = newBooking.CustomerId,
                        TableId = newBooking.TableId
                    });
                }
            }

            return BadRequest("No available tables match the booking request at the specified time.");
        }

        [HttpPut("UpdateBooking/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBooking(Guid id, CreateBookingDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            booking.Date = dto.Date;
            booking.Time = dto.Time;
            booking.Duration = dto.Duration;
            booking.GuestCount = dto.GuestCount;
            booking.TransactionId = dto.TransactionId;
            booking.CustomerId = dto.CustomerId;
            booking.TableId = dto.TableId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("DeleteBooking/{id}")]
        [Authorize (Roles = "admin")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpPatch("ChangeStatus/{id}")]
        [Authorize]
        public async Task<IActionResult> ChangeBookingStatus(Guid id, [FromBody] UpdateBookingStatusDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound("Booking not found");

            booking.Status = dto.Status;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "Booking status updated successfully",
                BookingId = booking.Id,
                NewStatus = booking.Status.ToString()
            });
        }
    }
}