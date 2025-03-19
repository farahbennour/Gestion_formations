using Microsoft.EntityFrameworkCore;

namespace Gestion_Formations.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
     
     
    }
    
    }

