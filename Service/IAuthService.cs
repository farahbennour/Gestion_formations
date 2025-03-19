using Gestion_Formations.Models;

namespace Gestion_Formations.Service
{
    public interface IAuthService
    {
        string Authenticate(string email, string password);
        bool RegisterUser(User user, string password);

    }
}
