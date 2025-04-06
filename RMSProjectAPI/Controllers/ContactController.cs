﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using System.Threading.Tasks;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitContactForm([FromBody] ContactForm contact)
        {
            if (!ModelState.IsValid || !contact.IsPrivacyPolicyAccepted)
            {
                return BadRequest(new { message = "Invalid data or privacy policy not accepted" });
            }

            try
            {
                await _context.ContactForms.AddAsync(contact);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Form submitted successfully",
                    contactId = contact.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving the form.", error = ex.Message });
            }
        }
    }
}