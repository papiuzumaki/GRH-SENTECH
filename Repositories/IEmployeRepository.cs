using GRH_SENTECH.Models;

namespace GRH_SENTECH.Repositories
{
    public interface IEmployeRepository : IRepository<Employe>
    {
        Task<IEnumerable<Employe>> GetAllWithDetailsAsync();
        Task<Employe?> GetByIdWithDetailsAsync(int id);
        Task<Employe?> GetByMatriculeAsync(string matricule);
        Task<IEnumerable<Employe>> GetByDepartementAsync(int departementId);
        Task<IEnumerable<Employe>> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
        Task<int> CountAsync(string? searchTerm = null);
    }
}
