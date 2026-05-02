using Microsoft.EntityFrameworkCore;
using GRH_SENTECH.Data;
using GRH_SENTECH.Models;
using GRH_SENTECH.Models.Enums;

namespace GRH_SENTECH.Repositories
{
    public class CongeRepository : Repository<Conge>, ICongeRepository
    {
        public CongeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Conge>> GetByEmployeAsync(int employeId)
        {
            return await _dbSet
                .Where(c => c.EmployeId == employeId)
                .OrderByDescending(c => c.DateDebut)
                .ToListAsync();
        }

        public async Task<int> GetTotalJoursAnnuelsAsync(int employeId, int annee)
        {
            var conges = await _dbSet
                .Where(c => c.EmployeId == employeId
                    && c.TypeConge == TypeConge.Annuel
                    && c.DateDebut.Year == annee
                    && c.Statut != StatutConge.Refuse)
                .ToListAsync();

            return conges.Sum(c => (c.DateFin - c.DateDebut).Days + 1);
        }

        public async Task<IEnumerable<Conge>> GetByStatutAsync(StatutConge statut)
        {
            return await _dbSet
                .Include(c => c.Employe)
                .Where(c => c.Statut == statut)
                .OrderBy(c => c.DateDebut)
                .ToListAsync();
        }
    }
}
