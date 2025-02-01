using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
using Wordapp;

var builder = WebApplication.CreateBuilder(args);

// Set the server to listen on port 5185
// builder.WebHost.UseUrls("http://localhost:5185");

// Register services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Add and configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true) // Needs to be changed to a specific origin in production
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // This is needed for SignalR to work
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS policy before routing
app.UseCors("AllowAll");

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

// Map SignalR hub
app.MapHub<GameHub>("/gameHub");

// Middleware to set or retrieve ClientId cookie
app.Use(async (context, next) =>
{
    const string clientIdCookieName = "ClientId";
    if (!context.Request.Cookies.TryGetValue(clientIdCookieName, out var clientId))
    {
        clientId = GenerateUniqueClientId();
        context.Response.Cookies.Append(clientIdCookieName, clientId, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,  // Enable for HTTPS
            SameSite = SameSiteMode.None,  // Required for cross-site access
            MaxAge = TimeSpan.FromDays(365)
        });
    }
    await next();
});

static string GenerateUniqueClientId()
{
    using var rng = RandomNumberGenerator.Create();
    var bytes = new byte[16];
    rng.GetBytes(bytes);
    return Convert.ToBase64String(bytes);
}

// Retrieves SignalR Hub context and instantiate Actions
var hubContext = app.Services.GetRequiredService<IHubContext<GameHub>>();
var actions = new Actions(app, hubContext);

try
{
    app.Run();
}
catch(Exception ex)
{
    Console.WriteLine("Unhandled exception: " + ex.ToString());
}
