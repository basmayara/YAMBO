using YamboAPI.Extensions;
using YamboAPI.Middleware;



var builder = WebApplication.CreateBuilder(args);

// Auto-reload config: when appsettings.json changes at runtime, IConfiguration reloads automatically
// This means if you update JwtSecret or connection string in env vars → no restart needed
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // reloadOnChange = auto-reload
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(); // Env vars override appsettings — used in production for secrets

//services
// Function chaining: each extension method returns IServiceCollection so we can chain them
builder.Services
    .AddDatabase(builder.Configuration)         //  EF Core + SQL Server (Scoped)
    .AddJwtAuthentication(builder.Configuration) //  JWT Bearer Auth
    .AddCorsPolicy()                             //  CORS — policy name: "AllowAll"
    .AddHttpCompression()                        // Brotli/Gzip response compression
    .AddCaching()                                //  In-memory cache (RAM)
    .AddControllers()                            //  MVC Controllers
    .Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

//monitoring
//  Health checks endpoint: GET /health → returns "Healthy" if API is running
// Used by Docker, Kubernetes, or any monitoring tool to check if service is alive
builder.Services.AddHealthChecks();

builder.WebHost.UseUrls("http://0.0.0.0:8080");


var app = builder.Build();


// Order matters in middleware pipeline!

// 1️⃣ Global exception handler — must be FIRST to catch errors from all other middleware
app.UseMiddleware<ExceptionMiddleware>();

// 2️⃣ HTTP compression — compress responses before sending
app.UseResponseCompression();

// 3️⃣ Swagger — only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4️⃣ CORS — allow cross-origin requests (use the named policy "AllowAll")
app.UseCors("AllowAll");

// 5️⃣ Authentication → Authorization (order is important!)
app.UseAuthentication();
app.UseAuthorization();

// 6️⃣ Health check endpoint for monitoring
//  Monitoring: visit GET /health to check if the API is alive
app.MapHealthChecks("/health");

// 7️⃣ Map controllers
app.MapControllers();

app.Run();