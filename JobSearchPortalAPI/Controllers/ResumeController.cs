using JobSearchPortalAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace JobSearchPortalAPI.Controllers
{
    [Route("api/resume")]
    [ApiController]
    public class ResumeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ResumeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateResume([FromBody] ResumeRequest resumeRequest)
        {
            if (resumeRequest == null || string.IsNullOrEmpty(resumeRequest.Template))
            {
                return BadRequest("Invalid resume request payload.");
            }

            var apiKey = _configuration.GetValue<string>("OpenAISetting:APIKey");
            var baseUrl = _configuration.GetValue<string>("OpenAISetting:BaseUrl");

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(baseUrl))
            {
                return StatusCode(500, "OpenAI API configuration is missing.");
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new OpenAIRequestDto
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<OpenAIMessageRequestDto>
                {
                    new OpenAIMessageRequestDto
                    {
                        Role = "system",
                        Content = "You are a professional resume generator."
                    },
                    new OpenAIMessageRequestDto
                    {
                        Role = "user",
                        Content = $"Generate a {{resumeRequest.Template}} resume based on the following details in ATS-friendly format: \r\nName: {{resumeRequest.Name}}, Experience Level: {{resumeRequest.ExperienceLevel}}, \r\nExperience: {{resumeRequest.Experience}}, Skills: {{string.Join(\", \", resumeRequest.Skills ?? new List<string>())}}, \r\nPreferred Job Type: {{resumeRequest.PreferredJobType}}, Education: {{resumeRequest.Education}}, \r\nCertifications: {{string.Join(\", \", resumeRequest.Certifications ?? new List<string>())}}, \r\nJob Requirements: {{resumeRequest.JobRequirements}}. \r\n\r\nThe output should ONLY contain a well-structured JSON object with these fields:\r\n- Name\r\n- Experiences (Array with Title, Company, Duration, Responsibilities)\r\n- ProfessionalSummary\r\n- Education (Object with Degree and Institution)\r\n- Certifications (Array of strings)\r\n- TechnicalSkills (Array of strings).\r\n"
                    }
                },
                Temperature = 0.7f,
                MaxTokens = 1500
            };

            try
            {
                var content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(baseUrl, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Parse the JSON response into a structured DTO
                    var openAiResponse = JsonConvert.DeserializeObject<OpenAIResponseDto>(responseContent);
                    var generatedContent = openAiResponse?.choices?[0]?.message?.content;

                    if (!string.IsNullOrEmpty(generatedContent))
                    {
                        var parsedJson = System.Text.Json.JsonSerializer.Deserialize<object>(generatedContent);
                        return Ok(new { message = "Resume generated successfully", resume = parsedJson });
                                           }

                    return StatusCode(500, "Failed to parse the generated resume.");
                }

                return StatusCode((int)response.StatusCode, responseContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
