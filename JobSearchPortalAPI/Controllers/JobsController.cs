using JobSearchPortalAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace JobSearchPortalAPI.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private static readonly HttpClient client = new HttpClient();

        // Inject configuration into constructor
        public JobsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Get LinkedIn access token using authorization code
        [HttpGet("linkedin/token")]
        public async Task<IActionResult> GetAccessToken(string code)
        {
            var clientId = _configuration["LinkedIn:ClientId"];
            var clientSecret = _configuration["LinkedIn:ClientSecret"];
            var redirectUri = _configuration["LinkedIn:RedirectUri"];

            var tokenUrl = "https://www.linkedin.com/oauth/v2/accessToken";
            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUri },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync(tokenUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Error getting access token.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var accessTokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
            var accessToken = accessTokenResponse["access_token"];

            return Ok(new { accessToken });
        }

        // Fetch jobs from LinkedIn using the access token
        [HttpGet("linkedin/jobs")]
        public async Task<IActionResult> GetLinkedInJobs([FromQuery] string accessToken)
        {
            var jobUrl = "https://api.linkedin.com/v2/jobs"; // LinkedIn API endpoint for jobs
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(jobUrl);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Error fetching LinkedIn jobs.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var linkedinJobs = JsonConvert.DeserializeObject<List<Job>>(content);

            return Ok(linkedinJobs);
        }

        // Example hardcoded job listings
        private static List<Job> jobListings = new List<Job>
        {
            new Job { Id = 1, Title = "Software Engineer", Company = "TechCorp", Location = "New York", SkillsRequired = new List<string> { "C#", " SQL" }, JobDescription = "Join our dynamic team to work on cutting-edge AI technologies that empower non-technical business users to generate and deploy code effortlessly. Backed by leading investors, we're in stealth mode, building something transformative. As a Staff Software Engineer specializing in back-end development, you'll design and develop high-performance systems, APIs, and distributed workflows, leveraging tools like AWS Lambda, DynamoDB, and Go to create scalable, reliable solutions. With at least 5 years of experience building complex production web apps, you'll bring expertise in serverless architectures, NoSQL databases, and cloud services, and a passion for using AI to enhance workflows. Join us in shaping the future of software development while delivering impactful solutions." },
            new Job { Id = 2, Title = "Frontend Developer", Company = "WebSolutions", Location = "San Francisco", SkillsRequired = new List<string> { "JavaScript", " React" }, JobDescription = "We are seeking a Front End Developer for a contract role in Woonsocket, RI, offering $70.00–$75.00/hr based on qualifications and experience. This position involves designing and implementing complex UI solutions using React, TypeScript, and Google Cloud Platform (GCP) in enterprise environments. Responsibilities include CI/CD deployment, ensuring application health, troubleshooting issues, and advocating best practices in security and accessibility. Candidates should have 7+ years of experience in frontend development, strong skills in responsive UI design, web security, and micro-frontends, and proficiency with tools like SASS/LESS and Web Components. Benefits include medical, dental, vision, 401K, and life insurance. Applicants must be legally authorized to work in the U.S.; sponsorship is unavailable." }
        };

        // Get hardcoded job listings
        [HttpGet]
        public IActionResult GetJobs()
        {
            return Ok(jobListings);
        }

        // Create a new job listing
        [HttpPost]
        public IActionResult CreateJob([FromBody] Job newJob)
        {
            newJob.Id = jobListings.Count + 1;
            jobListings.Add(newJob);
            return Ok(new { message = "Job created successfully", job = newJob });
        }

        // Update an existing job listing
        [HttpPut("{id}")]
        public IActionResult UpdateJob(int id, [FromBody] Job updatedJob)
        {
            var job = jobListings.FirstOrDefault(j => j.Id == id);
            if (job == null)
            {
                return NotFound("Job not found");
            }

            job.Title = updatedJob.Title;
            job.Company = updatedJob.Company;
            job.Location = updatedJob.Location;
            job.SkillsRequired = updatedJob.SkillsRequired;
            job.JobDescription = updatedJob.JobDescription;

            return Ok(new { message = "Job updated successfully" });
        }

        // Delete a job listing
        [HttpDelete("{id}")]
        public IActionResult DeleteJob(int id)
        {
            var job = jobListings.FirstOrDefault(j => j.Id == id);
            if (job == null)
            {
                return NotFound("Job not found");
            }

            jobListings.Remove(job);
            return Ok(new { message = "Job deleted successfully" });
        }
    }
}
