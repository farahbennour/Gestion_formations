using Gestion_Formations.Models;

namespace Gestion_Formations.Repertoires
{
    public interface IFormationRepository
    {
        Task AddFormation(Formation formation);
        Task<List<Formation>> GetAllFormationsAsync();
        Task SaveChangesAsync();
    }
}
