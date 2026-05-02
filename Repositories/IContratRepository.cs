using GRH_SENTECH.Models;

namespace GRH_SENTECH.Repositories
{
    public interface IContratRepository : IRepository<Contrat>
    {
        Task<Contrat?> GetContratActifAsync(int employeId);
        Task<IEnumerable<Contrat>> GetByEmployeAsync(int employeId);
    }
}
