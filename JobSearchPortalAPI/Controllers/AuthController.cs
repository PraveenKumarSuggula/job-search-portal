using JobSearchPortalAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace JobSearchPortalAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static List<User> users = new List<User>();

        public AuthController()
        {
            // Initialize sample users
            if (!users.Any())
            {
                users.Add(new User { Id = 1, Username = "john_doe", Email = "john@example.com", Password = "password123" });
                users.Add(new User { Id = 2, Username = "jane_doe", Email = "jane@example.com", Password = "password456" });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            if (users.Any(u => u.Username == registerRequest.Username))
            {
                return BadRequest("Username already exists");
            }

            var newUser = new User
            {
                Id = users.Count + 1,
                Username = registerRequest.Username,
                Password = registerRequest.Password,
                Email = registerRequest.Email
            };

            users.Add(newUser);
            return Ok(new { message = "Registration successful" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var user = users.FirstOrDefault(u => u.Username == loginRequest.Username && u.Password == loginRequest.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(user);
        }
    }
}

