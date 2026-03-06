using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Linq;
using YAMBO.Data;
using YAMBO.Model;
using YAMBO.Model.YAMBO.Model;
using BC = BCrypt.Net.BCrypt;

namespace YAMBO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ── LOGIN ──────────────────────────────────────────────
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { success = false, message = "Email ou password manquant." });

            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null || !BC.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { success = false, message = "Email ou password incorrect." });

            return Ok(new { success = true, message = "Login réussi!" });
        }

        // ── REGISTER ───────────────────────────────────────────
        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { success = false, message = "Email ou password manquant." });

            bool emailExists = _context.Users.Any(u => u.Email == request.Email);
            if (emailExists)
                return Conflict(new { success = false, message = "Email déjà utilisé." });

            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = BC.HashPassword(request.Password)
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new { success = true, message = "Compte créé!" });
        }

        // ── FORGOT PASSWORD ────────────────────────────────────
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
                return Ok(new { success = false, message = "Email not found." });

            string token = Guid.NewGuid().ToString();

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsUsed = false
            };

            _context.PasswordResetTokens.Add(resetToken);
            _context.SaveChanges();

            SendResetEmail(user.Email, token);

            return Ok(new { success = true, message = "Reset email sent!" });
        }

        // ── RESET PASSWORD ─────────────────────────────────────
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var resetToken = _context.PasswordResetTokens
                .FirstOrDefault(t => t.Token == request.Token && !t.IsUsed);

            if (resetToken == null)
                return Ok(new { success = false, message = "Invalid token." });

            if (resetToken.ExpiresAt < DateTime.UtcNow)
                return Ok(new { success = false, message = "Token expired." });

            var user = _context.Users.FirstOrDefault(u => u.Id == resetToken.UserId);
            if (user == null)
                return Ok(new { success = false, message = "User not found." });

            user.PasswordHash = BC.HashPassword(request.NewPassword);
            resetToken.IsUsed = true;
            _context.SaveChanges();

            return Ok(new { success = true, message = "Password reset successful!" });
        }

        // ── SEND EMAIL ─────────────────────────────────────────
        private void SendResetEmail(string toEmail, string token)
        {
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var appPassword = _configuration["EmailSettings:AppPassword"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("YAMBO", fromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "YAMBO - Reset Your Password";
            message.Body = new TextPart("plain")
            {
                Text = $"Your reset token is:\n\n{token}\n\nThis token expires in 1 hour."
            };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, false);
            smtp.Authenticate(fromEmail, appPassword);
            smtp.Send(message);
            smtp.Disconnect(true);
        }
    }

    // ── REQUEST MODELS ─────────────────────────────────────────
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}