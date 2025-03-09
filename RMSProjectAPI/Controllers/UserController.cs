using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RMSProjectAPI.Database;
using RMSProjectAPI.Database.Entity;
using RMSProjectAPI.Model;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RMSProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IConfiguration configuration,AppDbContext appDbContext,UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager) : ControllerBase
    {
        public AppDbContext _appDbContext = appDbContext;
        public UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IConfiguration _configuration = configuration;

        // ✅ Register API
        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserDto userDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser == null)
            {
                var user = new User
                {
                    Email = userDto.Email,
                    UserName = userDto.Email,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    BirthDate = userDto.BirthDate,
                    Gender = userDto.Gender,
                    Country = userDto.Country,
                    City = userDto.City,
                    Street = userDto.Street,

                    Status = userDto.Status,
                    Role = userDto.Role
                };
                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                await _userManager.AddToRoleAsync(user, "admin");

        }
        else
            {
                return BadRequest("User is exist");

            }

            return Ok(userDto);
        }

        // ✅ Login API
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

            var token = await GenerateJwtToken(user);
            return Ok(new { token });
        }

        // ✅ Add Role API
        //[HttpPost("AddRole")]
        //[Authorize(Roles = "admin")]
        //public async Task<IActionResult> AddRole([FromBody] string roleName)
        //{
        //    if (string.IsNullOrWhiteSpace(roleName))
        //    {
        //        return BadRequest("Role name cannot be empty.");
        //    }

        //    var roleExists = await _roleManager.RoleExistsAsync(roleName);
        //    if (roleExists)
        //    {
        //        return BadRequest("Role already exists.");
        //    }

        //    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        //    if (result.Succeeded)
        //    {
        //        return Ok($"Role '{roleName}' created successfully.");
        //    }

        //    return BadRequest("Failed to create role.");
        //}


        // ✅ Get All of Users

        [HttpGet("GetUsers")]
        [Authorize]
        public ActionResult GetAllUsers()
        {
            return Ok(_appDbContext.Users);
        }

        // ✅ JWT Token Generator
        private async Task<string> GenerateJwtToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // Add role claims
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

        // ✅ Get User by ID
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

        // ✅ Update User Information
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
            user.Country = updatedUser.Country;
            user.City = updatedUser.City;
            user.Street = updatedUser.Region;
            user.Status = updatedUser.Status;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User updated successfully.");
        }

        // ✅ Delete User
        [HttpDelete("DeleteUser/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User deleted successfully.");
        }

        // ✅ Change Password
        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password changed successfully.");
        }

        // ✅ Assign role to user
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

        // ✅ Refresh token
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

        // ================== Remaining APIs ==============
        // Email Verification
        // Forget Password
    }
}


// To Do

// 1- (Create (with generating QR Codes)/ Retrieve/ Update/ Delete)