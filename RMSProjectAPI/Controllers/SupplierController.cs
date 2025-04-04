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
    public class SupplierController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SupplierController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Add Supplier
        [HttpPost("add")]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierDto supplierDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var supervisor = await _context.Users.FindAsync(supplierDto.SupervisorId);
            if (supervisor == null)
                return NotFound("Supervisor not found");

            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Name = supplierDto.Name,
                Email = supplierDto.Email,
                Country = supplierDto.Country,
                City = supplierDto.City,
                Street = supplierDto.Street,
                SupervisorId = (Guid)supplierDto.SupervisorId
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Supplier added successfully", supplierId = supplier.Id });
        }

        // 2. Update Supplier
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateSupplier(Guid id, [FromBody] SupplierDto supplierDto)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return NotFound("Supplier not found");

            supplier.Name = supplierDto.Name ?? supplier.Name;
            supplier.Email = supplierDto.Email ?? supplier.Email;
            supplier.Country = supplierDto.Country ?? supplier.Country;
            supplier.City = supplierDto.City ?? supplier.City;
            supplier.Street = supplierDto.Street ?? supplier.Street;

            if (supplierDto.SupervisorId.HasValue)
            {
                var supervisor = await _context.Users.FindAsync(supplierDto.SupervisorId.Value);
                if (supervisor == null)
                    return NotFound("Supervisor not found");

                supplier.SupervisorId = supplierDto.SupervisorId.Value;
            }

            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Supplier updated successfully" });
        }

        // 3. Delete Supplier
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return NotFound("Supplier not found");

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Supplier deleted successfully" });
        }

        // 4. Get All Suppliers
        [HttpGet("all")]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var suppliers = await _context.Suppliers
                .Include(s => s.Supervisor)
                .ToListAsync();

            return Ok(suppliers);
        }

        // 5. Get Supplier by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(Guid id)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.Supervisor)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supplier == null)
                return NotFound("Supplier not found");

            return Ok(supplier);
        }

        // 6. Get Suppliers by Supervisor ID
        [HttpGet("by-supervisor/{supervisorId}")]
        public async Task<IActionResult> GetSuppliersBySupervisor(Guid supervisorId)
        {
            var suppliers = await _context.Suppliers
                .Where(s => s.SupervisorId == supervisorId)
                .Include(s => s.Supervisor)
                .ToListAsync();

            if (!suppliers.Any())
                return NotFound("No suppliers found for this supervisor");

            return Ok(suppliers);
        }
    }
}
