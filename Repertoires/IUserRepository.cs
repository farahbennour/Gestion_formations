using Gestion_Formations.Models;

namespace Gestion_Formations.Repertoires
{
    public interface IUserRepository
    {
        User GetByEmail(string Email);
        User GetById(int id);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Delete(User user);

    }
}
