using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SignInManager<User> _signInManager;


        public UserController(
            IConfiguration configuration,
            AppDbContext appDbContext,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            SignInManager<User> signInManager)
        {
            _configuration = configuration;
            _appDbContext = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

         //✅ Tested
        [HttpPost("Register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserDto userDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            var user = new User
            {
                Email = userDto.Email,
                UserName = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                BirthDate = userDto.BirthDate,
                Gender = userDto.Gender,
                Status = userDto.Status,
                ImagePath = userDto.ImagePath
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "admin");

            var roles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtToken(user);
            var response = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                Status = user.Status,
                ImagePath = user.ImagePath,
                Token = token,
                Roles = roles.ToList()
            };

            return Ok(response);
        }

        //[HttpPost("Register")]
        //public async Task<ActionResult<UserResponseDto>> Register(UserDto userDto)
        //{
        //    var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
        //    if (existingUser != null)
        //    {
        //        return BadRequest("User already exists");
        //    }

        //    var user = new User
        //    {
        //        Email = userDto.Email,
        //        UserName = userDto.Email,
        //        FirstName = userDto.FirstName,
        //        LastName = userDto.LastName,
        //        BirthDate = userDto.BirthDate,
        //        Gender = userDto.Gender,
        //        Country = userDto.Country,
        //        City = userDto.City,
        //        Street = userDto.Street,
        //        Status = userDto.Status,
        //        ImagePath = userDto.ImagePath,
        //        CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
        //    };

        //    var createResult = await _userManager.CreateAsync(user, userDto.Password);
        //    if (!createResult.Succeeded)
        //    {
        //        return BadRequest(createResult.Errors);
        //    }

        //    await _userManager.AddToRoleAsync(user, "customer");

        //    // Generate email confirmation token
        //    var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    var encodedToken = WebUtility.UrlEncode(emailToken);
        //    var confirmationLink = $"https://flavorhaven.runasp.net/api/auth/confirmemail?userId={user.Id}&token={encodedToken}";

        //    // Send confirmation email
        //    MailService.SendEmail(user.Email, "Confirm your email", $"Click here to confirm your email: {confirmationLink}");

        //    // Generate JWT token
        //    var jwtToken = await GenerateJwtToken(user);

        //    var roles = await _userManager.GetRolesAsync(user);
        //    var response = new UserResponseDto
        //    {
        //        Id = user.Id,
        //        Email = user.Email,
        //        UserName = user.UserName,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        BirthDate = user.BirthDate,
        //        Gender = user.Gender,
        //        Country = user.Country,
        //        City = user.City,
        //        Street = user.Street,
        //        Status = user.Status,
        //        ImagePath = user.ImagePath,
        //        Token = jwtToken,
        //        Roles = roles.ToList()
        //    };

        //    return Ok(response);
        //}

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return BadRequest("Invalid user ID");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return Ok("Email confirmed successfully!");

            return BadRequest("Email confirmation failed");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid credentials.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtToken(user);
            var response = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                Status = user.Status,
                ImagePath = user.ImagePath,
                Token = token,
                Roles = roles.ToList()
            };

            return Ok(response);
        }

        [HttpPost("AddRole")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> AddRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name cannot be empty.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                return BadRequest("Role already exists.");
            }

            var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            if (result.Succeeded)
            {
                return Ok($"Role '{roleName}' created successfully.");
            }

            return BadRequest("Failed to create role.");
        }

        [HttpGet("GetRoles")]
        [Authorize(Roles = "admin")]
        public ActionResult<IEnumerable<string>> GetRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return Ok(roles);
        }

        [HttpGet("GetUsers")]
        [Authorize(Roles = "admin")]
        public ActionResult GetAllUsers()
        {
            return Ok(_appDbContext.Users);
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims = claims.Append(new Claim(ClaimTypes.Role, role)).ToArray();
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpirationInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("GetUser/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }

        [HttpPut("UpdateUser/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto updatedUser)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.BirthDate = updatedUser.BirthDate;
            user.Gender = updatedUser.Gender;
            user.Status = updatedUser.Status;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User updated successfully.");
        }

        [HttpDelete("DeleteUser/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Remove user from all roles
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                var roleResult = await _userManager.RemoveFromRolesAsync(user, roles);
                if (!roleResult.Succeeded)
                {
                    return BadRequest(roleResult.Errors);
                }
            }

            // Now delete the user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User deleted successfully.");
        }


        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("Can't find it's id");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password changed successfully.");
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return Ok("If your email is registered and confirmed, a password reset link has been sent.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            var resetLink = $"https://flavorhaven.runasp.net/reset-password?email={user.Email}&token={encodedToken}";

            MailService.SendEmail(user.Email, "Reset Password", $"Click here to reset your password: {resetLink}");

            return Ok("If your email is registered and confirmed, a password reset link has been sent.");
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Invalid request.");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password has been reset successfully.");
        }

        [HttpPost("AssignRole")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.Role);
            if (!roleExists)
            {
                return BadRequest("Role does not exist.");
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok($"Role '{model.Role}' assigned to {user.Email}.");
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
        {
            var user = await _userManager.FindByEmailAsync(tokenDto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            var newToken = await GenerateJwtToken(user);
            return Ok(new { token = newToken });
        }

        [HttpPut("AddPhoneNumber/{id}")]
        [Authorize]
        public async Task<IActionResult> AddPhoneNumber(string id, [FromBody] string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return BadRequest("Phone number is required.");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            user.PhoneNumber = phoneNumber;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Phone number added/updated successfully.");
        }

        [HttpGet("GetUserAddresses/{userId}")]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetUserAddresses(Guid userId)
        {
            var addresses = await _appDbContext.Addresses
                .Where(a => a.UserId == userId)
                .Select(a => new AddressDto
                {
                    Id = a.Id,
                    Country = a.Country,
                    City = a.City,
                    Street = a.Street,
                    BuildingNumber = a.BuildingNumber,
                    Description = a.Description
                })
                .ToListAsync();

            return Ok(addresses);
        }

        [HttpPost("AddAddress/{userId}")]
        public async Task<IActionResult> AddAddress(Guid userId, [FromBody] CreateAddressDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found");

            var address = new Address
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Country = dto.Country,
                City = dto.City,
                Street = dto.Street,
                BuildingNumber = dto.BuildingNumber,
                Description = dto.Description
            };

            _appDbContext.Addresses.Add(address);
            await _appDbContext.SaveChangesAsync();

            return Ok("Address added successfully");
        }

        [HttpDelete("DeleteAddress/{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            var address = await _appDbContext.Addresses.FindAsync(id);
            if (address == null)
                return NotFound();

            _appDbContext.Addresses.Remove(address);
            await _appDbContext.SaveChangesAsync();
            return Ok("Address deleted successfully");
        }

        // ==== Testing Area ====
        //[HttpGet]
        //public ActionResult Get()
        //{
        //    MailService.SendEmail("abdulrahmanmamdouh789@gmail.com", "just to test the mail", "I'm Abdelrahman from flavor haven");
        //    return Ok();
        //}
    }
}