namespace Gestion_Formations.Repertoires
{
    using Gestion_Formations.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FormationRepository : IFormationRepository
    {
        private readonly ApplicationDbContext _context;

        public FormationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddFormation(Formation formation)
        {
            await _context.Formations.AddAsync(formation);
        }

        public async Task<List<Formation>> GetAllFormationsAsync()
        {
            return await _context.Formations.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
