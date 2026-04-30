using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
using YamboAPI.Data;
using YamboAPI.Model;
using BC = BCrypt.Net.BCrypt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Web;

namespace YamboAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuthController> _logger;

        
        public AuthController(
            AppDbContext context,
            IConfiguration configuration,
            IMemoryCache cache,
            ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _cache = cache;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            //  Basic validation — return early if input is missing
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { success = false, message = "Email ou password manquant." });

            //  Cache key naming convention: "login_attempt:{email}"
            // We use cache to prevent brute-force: block if too many failed attempts
            var cacheKey = $"login_attempt:{request.Email}";
            if (_cache.TryGetValue(cacheKey, out int attempts) && attempts >= 5)
            {
                // Security: lock account temporarily after 5 failed attempts
                _logger.LogWarning("Too many login attempts for email: {Email}", request.Email);
                return StatusCode(429, new { success = false, message = "Too many attempts. Try again later." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null || !BC.Verify(request.Password, user.PasswordHash))
            {
                // Increment failed attempt counter in cache (expires in 15 min)
                _cache.Set(cacheKey, attempts + 1, TimeSpan.FromMinutes(15));
                return Unauthorized(new { success = false, message = "Email ou password incorrect." });
            }

            //  Reset failed attempts on success
            _cache.Remove(cacheKey);

            //  Read JwtSecret from config — comes from env var or appsettings.Development.json
            // Auto-reload: if JwtSecret changes in config, IConfiguration picks it up without restart
            var jwtSecret = _configuration["JwtSecret"]
                ?? throw new InvalidOperationException("JwtSecret is not configured.");

            // Function chaining: fluent builder pattern for JWT token creation
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: new[] { new Claim(ClaimTypes.Email, user.Email!) },
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("User logged in: {Email}", request.Email);
            return Ok(new { success = true, token = tokenString, message = "Login réussi!" });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginRequest request)
        {
            //  Basic validation
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { success = false, message = "Email ou password manquant." });

            //  XSS: encode email before using it anywhere in output (HttpUtility.HtmlEncode)
            // Not needed here since we return JSON, but good habit if email is ever reflected back
            if (_context.Users.Any(u => u.Email == request.Email))
                return Conflict(new { success = false, message = "Email déjà utilisé." });

            var newUser = new User
            {
                Email = request.Email,
                // BCrypt hashes the password — never store plain text passwords
                PasswordHash = BC.HashPassword(request.Password)
            };

            // Transaction: SaveChanges() wraps the insert in a DB transaction automatically
            // If the insert fails, nothing is saved — data stays consistent
            _context.Users.Add(newUser);
            _context.SaveChanges();

            _logger.LogInformation("New user registered: {Email}", request.Email);
            return Ok(new { success = true, message = "Compte créé!" });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            // Security: even if email not found, return same response
            // This prevents "email enumeration" — attacker can't know if email exists
            if (user == null)
                return Ok(new { success = true, message = "Reset email sent!" });

            //  Cache key naming: "reset_token_sent:{email}" — tracks if we already sent a token
            var cacheKey = $"reset_token_sent:{request.Email}";
            if (_cache.TryGetValue(cacheKey, out _))
                return Ok(new { success = false, message = "Reset email already sent. Wait 1 minute." });

            // 🔐 Generate 6-digit token (consider using crypto-random in production)
            string token = new Random().Next(100000, 999999).ToString();

            // ✅ Transaction: both adding the token and saving happen atomically
            _context.PasswordResetTokens.Add(new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsUsed = false
            });
            _context.SaveChanges();

            //  Cache: prevent sending multiple emails in 1 minute (rate limiting via RAM cache)
            _cache.Set(cacheKey, true, TimeSpan.FromMinutes(1));

            SendResetEmail(user.Email, token);

            _logger.LogInformation("Password reset token sent to: {Email}", request.Email);
            return Ok(new { success = true, message = "Reset email sent!" });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            //  Eager loading not needed here — simple FirstOrDefault is sufficient
            // If we needed related data (e.g. user details), we'd use .Include() (eager loading)
            var resetToken = _context.PasswordResetTokens
                .FirstOrDefault(t => t.Token == request.Token && !t.IsUsed);

            if (resetToken == null)
                return Ok(new { success = false, message = "Invalid token." });

            if (resetToken.ExpiresAt < DateTime.UtcNow)
                return Ok(new { success = false, message = "Token expired." });

            var user = _context.Users.FirstOrDefault(u => u.Id == resetToken.UserId);
            if (user == null)
                return Ok(new { success = false, message = "User not found." });

            //  Hash new password before saving
            user.PasswordHash = BC.HashPassword(request.NewPassword);
            resetToken.IsUsed = true;

            //  Transaction: both changes (password + token mark) are saved together atomically
            // If one fails, neither is saved — prevents partial updates
            _context.SaveChanges();

            _logger.LogInformation("Password reset successful for user ID: {UserId}", user.Id);
            return Ok(new { success = true, message = "Password reset successful!" });
        }

        // Private helper method — not an endpoint, not static (needs _configuration)
        // If this were static, it couldn't access instance fields → keep as instance method
        private void SendResetEmail(string toEmail, string token)
        {
            //  Read secrets from IConfiguration — comes from env vars or appsettings.Development.json
            //  Auto-reload: if email settings change in config, they're picked up automatically
            var fromEmail = _configuration["EmailSettings:FromEmail"]
                ?? throw new InvalidOperationException("FromEmail is not configured.");
            var appPassword = _configuration["EmailSettings:AppPassword"]
                ?? throw new InvalidOperationException("AppPassword is not configured.");

            //  Function chaining: MimeMessage uses fluent builder-style property assignments
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("YAMBO", fromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "YAMBO - Reset Your Password";
            message.Body = new TextPart("plain")
            {
                // XSS: plain text email body — no HTML → no script injection possible
                Text = $"Your YAMBO reset code is:\n\n{token}\n\nThis code expires in 1 hour."
            };

            //  using statement: SmtpClient is disposed automatically after sending (IDisposable pattern)
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, false);
            smtp.Authenticate(fromEmail, appPassword);
            smtp.Send(message);
            smtp.Disconnect(true);
        }
    }
}