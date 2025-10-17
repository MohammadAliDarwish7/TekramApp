using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using TekramApp.Interfaces;
using TekramApp.Services.Auth;

namespace TekramApp.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsers _users;

        public AuthController(IUsers users)
        {
            _users = users;
        }

        // POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _users.LoginAsync(dto.Username, dto.Password);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid username or password");
            }
        }

        [HttpGet("GetCurrentUser")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
                return Unauthorized();

            // Convert all claims to a dictionary
            var claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);

            return Ok(claims);
        }
        public record LoginDto(string Username, string Password);
    }
}
