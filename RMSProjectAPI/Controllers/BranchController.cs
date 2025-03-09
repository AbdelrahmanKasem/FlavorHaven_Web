using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BranchController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Create Branch
        [HttpPost("CreateBranch")]
        public async Task<IActionResult> CreateBranch([FromBody] BranchDto branchDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var branch = new Branch
            {
                Id = Guid.NewGuid(),
                Name = branchDto.Name,
                Country = branchDto.Country,
                City = branchDto.City,
                Street = branchDto.Street,
                GoogleMapsLocation = branchDto.GoogleMapsLocation,
                ManagerId = branchDto.ManagerId
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBranchById), new { id = branch.Id }, branch);
        }

        // ✅ Get all Branch
        [HttpGet("GetBranches")]
        public async Task<IActionResult> GetAllBranches()
        {
            var branches = await _context.Branches.ToListAsync();
            return Ok(branches);
        }

        // ✅ Get Branch
        [HttpGet("GetBranch/{id}")]
        public async Task<IActionResult> GetBranchById(Guid id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                return NotFound();
            return Ok(branch);
        }

        // ✅ Update Branch
        [HttpPut("UpdateBranch/{id}")]
        public async Task<IActionResult> UpdateBranch(Guid id, [FromBody] BranchDto branchDto)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                return NotFound();

            branch.Name = branchDto.Name;
            branch.Country = branchDto.Country;
            branch.City = branchDto.City;
            branch.Street = branchDto.Street;
            branch.GoogleMapsLocation = branchDto.GoogleMapsLocation;
            branch.ManagerId = branchDto.ManagerId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Delete Branch
        [HttpDelete("DeleteBranch/{id}")]
        public async Task<IActionResult> DeleteBranch(Guid id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                return NotFound();

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
