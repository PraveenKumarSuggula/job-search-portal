using System.Collections.Generic;

namespace JobSearchPortalAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public string ExperienceLevel { get; set; }
        public string Experience { get; set; }
        public string PreferredJobType { get; set; }
        public string Education { get; set; }
        public List<string> Certifications { get; set; }
    }

    public class UserProfileUpdateDto
    {
        public string Email { get; set; }
        public List<string> Skills { get; set; }
        public string ExperienceLevel { get; set; }
        public string Experience { get; set; }
        public string PreferredJobType { get; set; }
        public string Education { get; set; }
        public List<string> Certifications { get; set; }
    }
}
