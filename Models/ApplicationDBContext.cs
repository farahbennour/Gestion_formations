using Microsoft.EntityFrameworkCore;

namespace Gestion_Formations.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Formation> Formations { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de la relation plusieurs à plusieurs entre User et Session via UserSession
            modelBuilder.Entity<UserSession>()
                .HasKey(us => new { us.UserId, us.SessionId });

            modelBuilder.Entity<UserSession>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSessions)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<UserSession>()
                .HasOne(us => us.Session)
                .WithMany(s => s.UserSessions)
                .HasForeignKey(us => us.SessionId);

            // Configuration de la relation 1:N entre Session et Formation
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Formation)
                .WithMany(f => f.Sessions)
                .HasForeignKey(s => s.FormationId);
        }
    }
}
