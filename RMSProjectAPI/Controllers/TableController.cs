//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using QRCoder;
//using RMSProjectAPI.Database;
//using RMSProjectAPI.Database.Entity;
//using RMSProjectAPI.Model;
//using System.Threading.Tasks;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

//[Route("api/tables")]
//[ApiController]
//public class TableController : ControllerBase
//{
//    private readonly AppDbContext _context;
//    private readonly TableRepository _tableRepository;

//    public TableController(TableRepository tableRepository, AppDbContext context)
//    {
//        _context = context;
//        _tableRepository = tableRepository;
//    }

//    [HttpPost]
//    public async Task<IActionResult> CreateTable([FromBody] TableDto tableDto)
//    {
//        var table = new Table
//        {
//            Id = Guid.NewGuid(),
//            IsAvailable = tableDto.IsAvailable,
//            Capacity = tableDto.Capacity,
//            BranchId = tableDto.BranchId
//        };

//        // Generate QR Code
//        var qrCodeUrl = $"{Request.Scheme}://{Request.Host}/api/Tables/DownloadQRCode/{table.Id}";
//        table.QrCodeUrl = qrCodeUrl;
//        table.QrCodeImage = GenerateQrCode(qrCodeUrl);

//        // Save QR code image to wwwroot
//        var filePath = Path.Combine(_env.WebRootPath, "qrcodes", $"{table.Id}.png");
//        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
//        await System.IO.File.WriteAllBytesAsync(filePath, table.QrCodeImage);

//        _context.Tables.Add(table);
//        await _context.SaveChangesAsync();

//        return CreatedAtAction(nameof(GetTableById), new { tableId = table.Id }, tableDto);
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetTables()
//    {
//        var tables = await _context.Tables.Select(t => new TableDto
//        {
//            Id = t.Id,
//            IsAvailable = t.IsAvailable,
//            Capacity = t.Capacity,
//            QrCodeUrl = t.QrCodeUrl,
//            BranchId = t.BranchId
//        }).ToListAsync();

//        return Ok(tables);
//    }

//    [HttpGet("{tableId}")]
//    public async Task<IActionResult> GetTableById(Guid tableId)
//    {
//        var table = await _context.Tables.FindAsync(tableId);
//        if (table == null) return NotFound();

//        return Ok(new TableDto
//        {
//            Id = table.Id,
//            IsAvailable = table.IsAvailable,
//            Capacity = table.Capacity,
//            QrCodeUrl = table.QrCodeUrl,
//            BranchId = table.BranchId
//        });
//    }

//    [HttpDelete("{tableId}")]
//    public async Task<IActionResult> DeleteTable(Guid tableId)
//    {
//        var table = await _context.Tables.FindAsync(tableId);
//        if (table == null) return NotFound();

//        _context.Tables.Remove(table);
//        await _context.SaveChangesAsync();

//        return NoContent();
//    }

//    [HttpGet("DownloadQRCode/{tableId}")]
//    public IActionResult DownloadQrCode(Guid tableId)
//    {
//        var filePath = Path.Combine(_env.WebRootPath, "qrcodes", $"{tableId}.png");

//        if (!System.IO.File.Exists(filePath))
//        {
//            return NotFound("QR Code not found.");
//        }

//        var fileBytes = System.IO.File.ReadAllBytes(filePath);
//        return File(fileBytes, "image/png", $"QRCode_{tableId}.png");
//    }

//    private byte[] GenerateQrCode(string text)
//    {
//        using var qrGenerator = new QRCodeGenerator();
//        using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
//        using var qrCode = new PngByteQRCode(qrCodeData);
//        return qrCode.GetGraphic(20);
//    }

//    // ✅ Add Table
//    //[HttpPost("AddTable")]
//    //public async Task<IActionResult> AddTable([FromBody] TableDto tableDto)
//    //{
//    //    if (tableDto == null)
//    //        return BadRequest(new { Message = "Invalid table data" });

//    //    var newTable = new Table
//    //    {
//    //        Id = Guid.NewGuid(),
//    //        Capacity = tableDto.Capacity,
//    //        BranchId = tableDto.BranchId,
//    //        QrCodeImage = null
//    //    };

//    //    await _context.Tables.AddAsync(newTable);
//    //    await _context.SaveChangesAsync();

//    //    return Ok(new { Message = "Table added successfully", TableId = newTable.Id });
//    //}

//    // ✅ Get Table by ID
//    //[HttpGet("GetTable/{tableId}")]
//    //public async Task<IActionResult> GetTable(Guid tableId)
//    //{
//    //    var table = await _context.Tables.FindAsync(tableId);
//    //    if (table == null)
//    //        return NotFound(new { Message = "Table not found" });

//    //    return Ok(table);
//    //}

//    // ✅ Get All Tables
//    //[HttpGet("GetAllTables")]
//    //public async Task<IActionResult> GetAllTables()
//    //{
//    //    var tables = await _context.Tables.ToListAsync();
//    //    return Ok(tables);
//    //}

//    // ✅ Update Table
//    //[HttpPut("UpdateTable/{tableId}")]
//    //public async Task<IActionResult> UpdateTable(Guid tableId, [FromBody] TableDto tableDto)
//    //{
//    //    var table = await _context.Tables.FindAsync(tableId);
//    //    if (table == null)
//    //        return NotFound(new { Message = "Table not found" });

//    //    table.Capacity = tableDto.Capacity;
//    //    table.BranchId = tableDto.BranchId;

//    //    _context.Tables.Update(table);
//    //    await _context.SaveChangesAsync();

//    //    return Ok(new { Message = "Table updated successfully" });
//    //}

//    // ✅ Delete Table
//    //[HttpDelete("DeleteTable/{tableId}")]
//    //public async Task<IActionResult> DeleteTable(Guid tableId)
//    //{
//    //    var table = await _context.Tables.FindAsync(tableId);
//    //    if (table == null)
//    //        return NotFound(new { Message = "Table not found" });

//    //    _context.Tables.Remove(table);
//    //    await _context.SaveChangesAsync();

//    //    return Ok(new { Message = "Table deleted successfully" });
//    //}

//    // ✅ Download QR Code
//    //[HttpGet("DownloadQRCode/{tableId}")]
//    //public IActionResult DownloadQrCode(Guid tableId)
//    //{
//    //    var table = _context.Tables.Find(tableId);
//    //    if (table == null || table.QrCodeImage == null)
//    //    {
//    //        return NotFound("QR Code not found.");
//    //    }

//    //    return File(table.QrCodeImage, "image/png", $"QRCode_{tableId}.png");
//    //}

//    // ✅ Book Table
//    [HttpPost("BookTable")]
//    public async Task<IActionResult> BookTable([FromBody] BookTableDto bookingDto)
//    {
//        if (bookingDto == null)
//            return BadRequest(new { Message = "Invalid booking data" });

//        var table = await _context.Tables.FindAsync(bookingDto.TableId);
//        if (table == null)
//            return NotFound(new { Message = "Table not found" });

//        var customer = await _context.Users.FindAsync(bookingDto.CustomerId);
//        if (customer == null)
//            return NotFound(new { Message = "Customer not found" });

//        // Check if the table is already booked for the same time
//        bool isAlreadyBooked = await _context.Bookings.AnyAsync(b =>
//            b.TableId == bookingDto.TableId &&
//            b.BookingTime == bookingDto.BookingTime &&
//            b.Status == BookingStatus.Confirmed);

//        if (isAlreadyBooked)
//            return BadRequest(new { Message = "Table is already booked for this time" });

//        var newBooking = new Booking
//        {
//            Id = Guid.NewGuid(),
//            TableId = bookingDto.TableId,
//            CustomerId = bookingDto.CustomerId,
//            BookingTime = bookingDto.BookingTime,
//            Date = DateTime.UtcNow,
//            Status = BookingStatus.Confirmed
//        };

//        await _context.Bookings.AddAsync(newBooking);
//        await _context.SaveChangesAsync();

//        return Ok(new { Message = "Table booked successfully", BookingId = newBooking.Id });
//    }

//    // ✅ Cancel Booking
//    [HttpPut("CancelBooking/{bookingId}")]
//    public async Task<IActionResult> CancelBooking(Guid bookingId)
//    {
//        var booking = await _context.Bookings.FindAsync(bookingId);
//        if (booking == null)
//            return NotFound(new { Message = "Booking not found" });

//        if (booking.Status == BookingStatus.Cancelled)
//            return BadRequest(new { Message = "Booking is already cancelled" });

//        booking.Status = BookingStatus.Cancelled;
//        _context.Bookings.Update(booking);
//        await _context.SaveChangesAsync();

//        return Ok(new { Message = "Booking cancelled successfully" });
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Model;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

[Route("api/tables")]
[ApiController]
public class TableController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TableRepository _tableRepository;

    public TableController(TableRepository tableRepository, AppDbContext context)
    {
        _context = context;
        _tableRepository = tableRepository;
    }

    // ✅ Add Table
    [HttpPost("AddTable")]
    public async Task<IActionResult> AddTable([FromBody] TableDto tableDto)
    {
        if (tableDto == null || tableDto.Capacity <= 0)
        {
            return BadRequest(new { Message = "Invalid table capacity" });
        }

        await _tableRepository.AddTableAsync(tableDto.Capacity);
        return Ok(new { Message = "Table added successfully" });
    }

    // ✅ Get All Tables
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllTables()
    {
        var tables = await _context.Tables.ToListAsync();
        return Ok(tables);
    }

    // ✅ Get Table By ID
    [HttpGet("GetById/{tableId}")]
    public async Task<IActionResult> GetTableById(Guid tableId)
    {
        var table = await _context.Tables.FindAsync(tableId);
        if (table == null)
        {
            return NotFound(new { Message = "Table not found" });
        }
        return Ok(table);
    }

    // ✅ Update Table
    [HttpPut("Update/{tableId}")]
    public async Task<IActionResult> UpdateTable(Guid tableId, [FromBody] TableDto tableDto)
    {
        var table = await _context.Tables.FindAsync(tableId);
        if (table == null)
        {
            return NotFound(new { Message = "Table not found" });
        }

        if (tableDto.Capacity <= 0)
        {
            return BadRequest(new { Message = "Invalid table capacity" });
        }

        table.Capacity = tableDto.Capacity;
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Table updated successfully" });
    }

    // ✅ Delete Table
    [HttpDelete("Delete/{tableId}")]
    public async Task<IActionResult> DeleteTable(Guid tableId)
    {
        var table = await _context.Tables.FindAsync(tableId);
        if (table == null)
        {
            return NotFound(new { Message = "Table not found" });
        }

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Table deleted successfully" });
    }

    // ✅ Download QR Code
    [HttpGet("DownloadQRCode/{tableId}")]
    public IActionResult DownloadQrCode(Guid tableId)
    {
        var table = _context.Tables.Find(tableId);
        if (table == null || table.QrCodeImage == null)
        {
            return NotFound("QR Code not found.");
        }

        return File(table.QrCodeImage, "image/png", $"QRCode_{tableId}.png");
    }
}