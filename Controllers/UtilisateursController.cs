using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Yambo_API.Model;

namespace Yambo_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilisateursController : ControllerBase
    {
        private readonly GameDbContext _context;

        public UtilisateursController(GameDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register(Utilisateur Utilisateur)
        {
            _context.Utilisateurs.Add(Utilisateur);
            _context.SaveChanges();
            return Ok("User registered");
        }

        [HttpPost("login")]
        public IActionResult Login(Utilisateur Utilisateur)
        {
            var existingUser = _context.Utilisateurs
                .FirstOrDefault(u => u.Email == Utilisateur.Email && u.MotDePasse == Utilisateur.MotDePasse);

            if (existingUser == null)
                return Unauthorized("Invalid credentials");

            return Ok("Login success");
        }
    }
}
