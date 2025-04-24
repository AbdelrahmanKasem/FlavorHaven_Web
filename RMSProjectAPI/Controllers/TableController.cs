using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.DTOs;
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

    //// ✅ Add Table
    //[HttpPost("AddTable")]
    //public async Task<IActionResult> AddTable([FromBody] TableDto tableDto)
    //{
    //    if (tableDto == null || tableDto.Capacity <= 0)
    //    {
    //        return BadRequest(new { Message = "Invalid table capacity" });
    //    }

    //    await _tableRepository.AddTableAsync(tableDto.Capacity);
    //    return Ok(new { Message = "Table added successfully" });
    //}

    //public async Task AddTableAsync(int capacity)
    //{
    //    var lastTableNumber = await _context.Tables
    //        .OrderByDescending(t => t.TableNumber)
    //        .Select(t => t.TableNumber)
    //        .FirstOrDefaultAsync();

    //    var newTable = new Table
    //    {
    //        Id = Guid.NewGuid(),
    //        TableNumber = lastTableNumber + 1,
    //        Capacity = capacity,
    //        IsAvailable = true,
    //        QrCodeUrl = "", // Optional: generate based on table info
    //        QrCodeImage = null
    //    };

    //    _context.Tables.Add(newTable);
    //    await _context.SaveChangesAsync();
    //}


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

    [HttpGet("TableStatus")]
    public async Task<ActionResult<object>> GetTablesWithStatus()
    {
        var now = DateTime.Now;

        var tables = await _context.Tables
            .Include(t => t.Bookings)
            .ToListAsync();

        int occupiedCount = 0;
        List<TableStatusDto> tableStatusList = new();

        foreach (var table in tables)
        {
            string status = "Not Booked";

            foreach (var booking in table.Bookings)
            {
                var start = booking.Date.Date + booking.Time;
                var end = start + booking.Duration;

                if (now >= start && now < end)
                {
                    status = "Booked (customers are on the table)";
                    occupiedCount++;
                    break;
                }
                else if (now < start)
                {
                    status = "Booked (booking time hasn't arrived)";
                }
            }

            tableStatusList.Add(new TableStatusDto
            {
                TableId = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                Status = status
            });
        }

        return Ok(new
        {
            TotalTables = tables.Count,
            OccupiedTables = occupiedCount,
            Tables = tableStatusList
        });
    }

    [HttpGet("ReadyOrderForTables")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllReadyOrdersFromTables()
    {
        var readyOrders = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItemSize)
            .Where(o => o.TableId != null && o.Status == OrderStatus.Ready)
            .ToListAsync();

        var orderDtos = readyOrders.Select(order => new OrderDto
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            Status = order.Status,
            Type = order.Type,
            Price = order.Price,
            Latitude = order.Latitude,
            Longitude = order.Longitude,
            Address = order.Address,
            PaymentSystem = order.PaymentSystem,
            TransactionId = order.TransactionId,
            Note = order.Note,
            CustomerId = order.CustomerId,
            TableId = order.TableId,
            EstimatedPreparationTime = order.EstimatedPreparationTime,
            OrderItems = order.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                Quantity = oi.Quantity,
                Note = oi.Note,
                SpicyLevel = oi.SpicyLevel,
                Price = oi.Price,
                MenuItemId = oi.MenuItemId,
                MenuItemName = oi.MenuItem.Name,
                MenuItemSizeId = oi.MenuItemSizeId,
                MenuItemSizePrice = oi.MenuItemSize.Price
            }).ToList()
        }).ToList();

        return Ok(orderDtos);
    }
}