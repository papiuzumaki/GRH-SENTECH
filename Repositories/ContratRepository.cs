using Microsoft.EntityFrameworkCore;
using GRH_SENTECH.Data;
using GRH_SENTECH.Models;
using GRH_SENTECH.Models.Enums;

namespace GRH_SENTECH.Repositories
{
    public class ContratRepository : Repository<Contrat>, IContratRepository
    {
        public ContratRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Contrat?> GetContratActifAsync(int employeId)
        {
            return await _dbSet
                .Where(c => c.EmployeId == employeId && (c.DateFin == null || c.DateFin >= DateTime.Today))
                .OrderByDescending(c => c.DateDebut)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Contrat>> GetByEmployeAsync(int employeId)
        {
            return await _dbSet
                .Where(c => c.EmployeId == employeId)
                .OrderByDescending(c => c.DateDebut)
                .ToListAsync();
        }
    }
}
