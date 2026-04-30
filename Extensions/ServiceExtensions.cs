using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YamboAPI.Data;

namespace YamboAPI.Extensions
{
    // Extension class — keeps Program.cs clean
    // Instead of registering everything in Program.cs, we group them here
    // This is called the "Extension Method" pattern for clean architecture
    public static class ServiceExtensions
    {
        // Extension method on IServiceCollection — called as builder.Services.AddDatabase(...)
        // This is a "fluent/function chain" style: each method returns the same object so you can chain calls
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            //  AddDbContext uses Scoped lifetime by default:
            // A new AppDbContext is created per HTTP request — correct for EF Core
            //  Dependency Injection: AppDbContext is injected wherever needed (e.g. AuthController)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            return services; //  Function chaining: returns services so caller can chain more calls
        }

        //  Extension method to register JWT Authentication
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            //  Read JwtSecret from config (comes from env variable or appsettings.Development.json)
            //  Auto-reload: IConfiguration automatically picks up changes when reloadOnChange=true (set in Program.cs)
            var jwtSecret = config["JwtSecret"]
                ?? throw new InvalidOperationException("JwtSecret is not configured.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            return services; // Function chaining
        }

        //  Extension method to register CORS policy
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            //  CORS: allows frontend from any origin to call this API
            // Cache name: "AllowAll" — clear name describes what the policy does
            services.AddCors(o => o.AddPolicy("AllowAll", b =>
                b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            return services; //  Function chaining
        }

        // Extension method to register HTTP compression
        public static IServiceCollection AddHttpCompression(this IServiceCollection services)
        {
            // Response Compression: reduces response size sent to browser/client
            // Supports Brotli (best) and Gzip — browser sends "Accept-Encoding" header
            // and server responds with compressed body → faster load times
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true; //olso compress HTTPS responses
            });

            return services; // Function chaining
        }

        // Extension method to register RAM caching
        public static IServiceCollection AddCaching(this IServiceCollection services)
        {
            //  In-memory cache: stores data in RAM for fast access
            // Use this for: token blacklists, rate limiting counters, frequently-read configs
            // Cache key naming convention used in this project: "user:{id}", "token:{value}"
            services.AddMemoryCache();

            return services; // Function chaining
        }
    }
}