 using Microsoft.EntityFrameworkCore;
using Yambo_API.Model;
namespace Yambo_API
{

    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options) { }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
    }
}
