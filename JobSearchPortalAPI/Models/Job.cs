using System.Collections.Generic;

namespace JobSearchPortalAPI.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public List<string> SkillsRequired { get; set; } = new List<string>();
        public string JobDescription { get; set; }
    }
}
