using GRH_SENTECH.Models;
using GRH_SENTECH.Models.Enums;

namespace GRH_SENTECH.Repositories
{
    public interface ICongeRepository : IRepository<Conge>
    {
        Task<IEnumerable<Conge>> GetByEmployeAsync(int employeId);
        Task<int> GetTotalJoursAnnuelsAsync(int employeId, int annee);
        Task<IEnumerable<Conge>> GetByStatutAsync(StatutConge statut);
    }
}
