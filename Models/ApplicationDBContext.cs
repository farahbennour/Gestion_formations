using Microsoft.EntityFrameworkCore;

namespace Gestion_Formations.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Formation> Formations { get; set; }

        // Supprimer les tables UserSession et Session
        // public DbSet<Session> Sessions { get; set; } 
        // public DbSet<UserSession> UserSessions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
         
        }
    }
}
