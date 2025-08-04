using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelegramClone.Api.Data;
using TelegramClone.Api.Models;

namespace TelegramClone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User Registered");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == userDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
                return Unauthorized();

            // TODO: Return JWT Token (bước sau)
            return Ok("Login Success");
        }
    }
}
