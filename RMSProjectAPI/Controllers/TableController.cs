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


// Book a table
// Cancel booking