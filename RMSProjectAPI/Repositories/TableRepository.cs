//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using RMSProjectAPI.Database;
//using RMSProjectAPI.Database.Entity;

//public class TableRepository
//{
//    private readonly AppDbContext _context;

//    public TableRepository(AppDbContext context)
//    {
//        _context = context;
//    }

//    private readonly QRCodeService _qrCodeService;

//    private readonly ILogger<TableRepository> _logger;

//    public TableRepository(AppDbContext context, QRCodeService qrCodeService, ILogger<TableRepository> logger)
//    {
//        _context = context;
//        _qrCodeService = qrCodeService;
//        _logger = logger;
//    }

//    public async Task AddTableAsync(int capacity)
//    {
//        try
//        {
//            //string qrCodeUrl = $"https://www.linkedin.com/in/abdelrahman-mamdouh-cs/";
//            string qrCodeUrl = $"https://flavorhaven.runasp.net/menu?table={Guid.NewGuid()}";
//            byte[] qrCodeImage = _qrCodeService.GenerateQRCode(qrCodeUrl);

//            var table = new Table
//            {
//                Id = Guid.NewGuid(),
//                IsAvailable = true,
//                Capacity = capacity,
//                QrCodeUrl = qrCodeUrl,
//                QrCodeImage = qrCodeImage
//            };

//            await _context.Tables.AddAsync(table);
//            await _context.SaveChangesAsync();
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error adding table.");
//            throw;
//        }
//    }
//}


using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;

public class TableRepository
{
    private readonly AppDbContext _context;

    public TableRepository(AppDbContext context)
    {
        _context = context;
    }

    private readonly QRCodeService _qrCodeService;

    private readonly ILogger<TableRepository> _logger;

    public TableRepository(AppDbContext context, QRCodeService qrCodeService, ILogger<TableRepository> logger)
    {
        _context = context;
        _qrCodeService = qrCodeService;
        _logger = logger;
    }

    public async Task AddTableAsync(int capacity)
    {
        try
        {
            int lastTableNumber = await _context.Tables
                .OrderByDescending(t => t.TableNumber)
                .Select(t => t.TableNumber)
                .FirstOrDefaultAsync();

            int newTableNumber = lastTableNumber + 1;

            string qrCodeUrl = $"https://flavorhaven.runasp.net/menu?table={newTableNumber}";
            byte[] qrCodeImage = _qrCodeService.GenerateQRCode(qrCodeUrl);

            var table = new Table
            {
                TableNumber = newTableNumber,
                IsAvailable = true,
                Capacity = capacity,
                QrCodeUrl = qrCodeUrl,
                QrCodeImage = qrCodeImage
            };

            await _context.Tables.AddAsync(table);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding table.");
            throw;
        }
    }
}

