using Microsoft.AspNetCore.Mvc;
using YAMBO.Data;
using System.Linq;

namespace YAMBO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == request.Email && u.PasswordHash == request.Password);

            if (user == null)
                return Ok(false); // login fail

            return Ok(true);      // login success
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}