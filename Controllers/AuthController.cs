using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelegramClone.Api.Data;
using TelegramClone.Api.Models;
using TelegramClone.Api.Services;

namespace TelegramClone.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
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

            var token = _tokenService.GenerateToken(user.Username);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("friends")]
        public IActionResult GetFriends()
        {
            return Ok("Get data successfully");
        }

        [HttpGet("{user1}/{user2}")]
        public async Task<IActionResult> GetMessage(string user1, string user2)
        {
            var messages = await _context.Messages
                .Where(m => (m.Sender == user1 && m.Receiver == user2) ||
                            (m.Sender == user2 && m.Receiver == user2))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Ok(messages);
        }
    }
}
