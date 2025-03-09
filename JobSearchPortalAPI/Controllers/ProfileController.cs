using JobSearchPortalAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace JobSearchPortalAPI.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private static List<User> users = AuthController.users;

        [HttpGet("{username}")]
        public IActionResult GetUserProfile(string username)
        {
            var user = users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }

        [HttpPut("{username}")]
        public IActionResult UpdateUserProfile(string username, [FromBody] UserProfileUpdateDto updatedUser)
        {
            var user = users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.Email = updatedUser.Email;
            user.Skills = updatedUser.Skills;
            user.ExperienceLevel = updatedUser.ExperienceLevel;
            user.Experience = updatedUser.Experience;
            user.PreferredJobType = updatedUser.PreferredJobType;
            user.Education = updatedUser.Education;
            user.Certifications = updatedUser.Certifications;

            return Ok(new { message = "Profile updated successfully" });
        }

        [HttpDelete("{username}")]
        public IActionResult DeleteUserProfile(string username)
        {
            var user = users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            users.Remove(user);
            return Ok(new { message = "Profile deleted successfully" });
        }
    }
}
