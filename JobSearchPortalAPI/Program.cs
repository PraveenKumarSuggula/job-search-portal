using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Explicitly bind environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

// Manually override appsettings.json values with environment variables
var openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ??
                   builder.Configuration["OpenAISetting:APIKey"];

var linkedInClientId = Environment.GetEnvironmentVariable("LINKEDIN_CLIENT_ID") ??
                        builder.Configuration["LinkedIn:ClientId"];

var linkedInClientSecret = Environment.GetEnvironmentVariable("LINKEDIN_CLIENT_SECRET") ??
                            builder.Configuration["LinkedIn:ClientSecret"];

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
    options.ClientId = linkedInClientId;
    options.ClientSecret = linkedInClientSecret;
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

// Use CORS policy
app.UseCors("AllowOrigin");

// Authentication and Authorization Middleware
app.UseAuthentication();  // Make sure authentication is enabled
app.UseAuthorization();

// Configure Swagger for development environment
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger"; // Serve Swagger at /swagger
    });
}

app.UseHttpsRedirection();

// Map Controllers
app.MapControllers();

app.Run();
