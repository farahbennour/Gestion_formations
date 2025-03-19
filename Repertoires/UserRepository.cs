using Gestion_Formations.Models;

namespace Gestion_Formations.Repertoires
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public User GetByEmail(string email)
        {

            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
