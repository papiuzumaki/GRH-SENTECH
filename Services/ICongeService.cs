using GRH_SENTECH.Models;
using GRH_SENTECH.Models.Enums;

namespace GRH_SENTECH.Services
{
    public interface ICongeService
    {
        Task<ServiceResult<IEnumerable<Conge>>> GetByEmployeAsync(int employeId);
        Task<ServiceResult<Conge>> DemanderCongeAsync(int employeId, Conge conge);
        Task<ServiceResult<Conge>> ChangerStatutAsync(int congeId, StatutConge nouveauStatut);
    }
}
