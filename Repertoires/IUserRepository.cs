using Gestion_Formations.Models;

namespace Gestion_Formations.Repertoires
{
    public interface IUserRepository
    {
        User GetByEmail(string Email);
       
        void Add(User user);
    }
}
