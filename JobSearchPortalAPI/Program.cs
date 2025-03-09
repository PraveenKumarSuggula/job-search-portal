using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

// OAuth 2.0 Authentication for LinkedIn
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "LinkedIn";
})
.AddCookie()
.AddOAuth("LinkedIn", options =>
{
    options.ClientId = builder.Configuration["LinkedIn:ClientId"];
    options.ClientSecret = builder.Configuration["LinkedIn:ClientSecret"];
    options.CallbackPath = new PathString("/signin-linkedin");

    options.AuthorizationEndpoint = "https://www.linkedin.com/oauth/v2/authorization";
    options.TokenEndpoint = "https://www.linkedin.com/oauth/v2/accessToken";
    options.UserInformationEndpoint = "https://api.linkedin.com/v2/me";

    options.Scope.Add("r_liteprofile");
    options.Scope.Add("r_emailaddress");

    options.SaveTokens = true;

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var accessToken = context.AccessToken;

            // Fetch LinkedIn user data
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.linkedin.com/v2/me");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();

            var user = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

            // Add claims to the identity
            context.Identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.localizedFirstName.ToString()));
            context.Identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.id.ToString()));
        }
    };
});

var app = builder.Build();

// Retrieve secrets from environment variables
var openAiApiKey = builder.Configuration["OpenAISetting:APIKey"] ?? 
                   Environment.GetEnvironmentVariable("OPENAI_API_KEY");

var linkedInClientId = builder.Configuration["LinkedIn:ClientId"] ?? 
                        Environment.GetEnvironmentVariable("LINKEDIN_CLIENT_ID");

var linkedInClientSecret = builder.Configuration["LinkedIn:ClientSecret"] ?? 
                            Environment.GetEnvironmentVariable("LINKEDIN_CLIENT_SECRET");
                            
// Use CORS policy
app.UseCors("AllowOrigin");

// Authentication and Authorization Middleware
app.UseAuthentication();  // Make sure authentication is enabled
app.UseAuthorization();

// Configure Swagger for development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map Controllers
app.MapControllers();

app.Run();
