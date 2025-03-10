using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Load appsettings.json & override with environment variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();  // Ensure Azure env variables override settings

// Register IConfiguration for Dependency Injection
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Resume API", Version = "v1" });
});

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowOrigin");
app.UseAuthentication();
app.UseAuthorization();

// Swagger Setup
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Resume API V1"); });
}

app.UseHttpsRedirection();
app.MapControllers();

// Debug Logging to Check Environment Variables in Azure Logs
Console.WriteLine($"[DEBUG] OPENAI_API_KEY = {Environment.GetEnvironmentVariable("OPENAI_API_KEY")}");
Console.WriteLine($"[DEBUG] LINKEDIN_CLIENT_ID = {Environment.GetEnvironmentVariable("LINKEDIN_CLIENT_ID")}");
Console.WriteLine($"[DEBUG] LINKEDIN_CLIENT_SECRET = {Environment.GetEnvironmentVariable("LINKEDIN_CLIENT_SECRET")}");

app.Run();
