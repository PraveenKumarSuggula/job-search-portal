using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using JobSearchPortalAPI.Models;

namespace JobSearchPortalAPI.Controllers
{
    [Route("api/resume")]
    [ApiController]
    public class ResumeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _openAiApiKey;
        private readonly string _openAiBaseUrl;

        public ResumeController(IConfiguration configuration)
        {
            _configuration = configuration;

            // Retrieve environment variables first, fallback to appsettings.json
            _openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? _configuration["OpenAISetting:APIKey"];

            _openAiBaseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL")
                ?? _configuration["OpenAISetting:BaseUrl"]
                ?? "https://api.openai.com/v1/chat/completions";

            // Debugging to verify values in Azure logs
            Console.WriteLine($"[DEBUG] Retrieved OPENAI_API_KEY: {_openAiApiKey}");
            Console.WriteLine($"[DEBUG] Retrieved OPENAI_BASE_URL: {_openAiBaseUrl}");
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateResume([FromBody] ResumeRequest resumeRequest)
        {
            if (resumeRequest == null || string.IsNullOrEmpty(resumeRequest.Template))
            {
                return BadRequest("Invalid resume request payload.");
            }

            if (string.IsNullOrEmpty(_openAiApiKey))
            {
                return StatusCode(500, "OpenAI API Key is missing from environment variables and appsettings.json.");
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

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
                        Content = $"Generate a {resumeRequest.Template} resume with these details: \n" +
                                  $"Name: {resumeRequest.Name}, Experience Level: {resumeRequest.ExperienceLevel}, \n" +
                                  $"Experience: {resumeRequest.Experience}, Skills: {string.Join(", ", resumeRequest.Skills ?? new List<string>())}, \n" +
                                  $"Preferred Job Type: {resumeRequest.PreferredJobType}, Education: {resumeRequest.Education}, \n" +
                                  $"Certifications: {string.Join(", ", resumeRequest.Certifications ?? new List<string>())}, \n" +
                                  $"Job Requirements: {resumeRequest.JobRequirements}. \n\n" +
                                  $"The output must be a structured JSON object with: Name, Experiences (array), " +
                                  $"ProfessionalSummary, Education, Certifications, and TechnicalSkills."
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

                var response = await client.PostAsync(_openAiBaseUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
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
