using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;
using RMSProjectAPI.Services;

namespace RMSProjectAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PhoneNumbersController : ControllerBase
    {
        private readonly IPhoneNumberService _phoneNumberService;
        private readonly UserManager<User> _userManager;

        public PhoneNumbersController(
            IPhoneNumberService phoneNumberService,
            UserManager<User> userManager)
        {
            _phoneNumberService = phoneNumberService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhoneNumber>>> GetPhoneNumbers()
        {
            var userId = Guid.Parse(_userManager.GetUserId(User));
            var phoneNumbers = await _phoneNumberService.GetUserPhoneNumbersAsync(userId);
            return Ok(phoneNumbers);
        }

        [HttpPost]
        public async Task<ActionResult<PhoneNumber>> AddPhoneNumber(PhoneNumberDto phoneNumberDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = Guid.Parse(_userManager.GetUserId(User));

            try
            {
                var phoneNumber = await _phoneNumberService.AddPhoneNumberAsync(userId, phoneNumberDto);
                return CreatedAtAction(nameof(GetPhoneNumbers), new { id = phoneNumber.Id }, phoneNumber);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}