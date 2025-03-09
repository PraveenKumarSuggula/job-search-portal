using System.Collections.Generic;

namespace JobSearchPortalAPI.Models
{
    public class ResumeRequest
    {
        public string Name { get; set; }
        public string Template { get; set; }
        public string ExperienceLevel { get; set; }
        public string Experience { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public string PreferredJobType { get; set; }
        public string Education { get; set; }
        public List<string> Certifications { get; set; } = new List<string>();

        public string JobRequirements { get; set; }
    }

    public class ParsedResumeDto
    {
        public string Name { get; set; }
        public List<string> Experiences { get; set; }
        public string ProfessionalSummary { get; set; }
        public string Education { get; set; }
        public List<string> Certifications { get; set; }
        public List<string> TechnicalSkills { get; set; }
    }

}
