using Microsoft.EntityFrameworkCore;
using GRH_SENTECH.Data;
using GRH_SENTECH.Models;

namespace GRH_SENTECH.Repositories
{
    public class EmployeRepository : Repository<Employe>, IEmployeRepository
    {
        public EmployeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Employe>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(e => e.Departement)
                .Include(e => e.Poste)
                .Include(e => e.Contrats)
                .ToListAsync();
        }

        public async Task<Employe?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Departement)
                .Include(e => e.Poste)
                .Include(e => e.Contrats)
                .Include(e => e.Evaluations)
                .Include(e => e.Conges)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employe?> GetByMatriculeAsync(string matricule)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Matricule == matricule);
        }

        public async Task<IEnumerable<Employe>> GetByDepartementAsync(int departementId)
        {
            return await _dbSet
                .Include(e => e.Poste)
                .Where(e => e.DepartementId == departementId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employe>> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
        {
            var query = _dbSet
                .Include(e => e.Departement)
                .Include(e => e.Poste)
                .Include(e => e.Contrats)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e =>
                    e.Nom.Contains(searchTerm) ||
                    e.Prenom.Contains(searchTerm) ||
                    e.Matricule.Contains(searchTerm) ||
                    e.Email.Contains(searchTerm));
            }

            return await query
                .OrderBy(e => e.Nom)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(string? searchTerm = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e =>
                    e.Nom.Contains(searchTerm) ||
                    e.Prenom.Contains(searchTerm) ||
                    e.Matricule.Contains(searchTerm) ||
                    e.Email.Contains(searchTerm));
            }

            return await query.CountAsync();
        }
    }
}
