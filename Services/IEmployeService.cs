using GRH_SENTECH.Models;

namespace GRH_SENTECH.Services
{
    public interface IEmployeService
    {
        Task<ServiceResult<IEnumerable<Employe>>> GetAllAsync(int page, int pageSize, string? search = null);
        Task<ServiceResult<Employe>> GetByIdAsync(int id);
        Task<ServiceResult<Employe>> CreerEmployeAsync(Employe employe);
        Task<ServiceResult<Employe>> ModifierEmployeAsync(Employe employe);
        Task<ServiceResult<bool>> SupprimerEmployeAsync(int id);
        Task<ServiceResult<Contrat>> AjouterContratAsync(int employeId, Contrat contrat);
        Task<ServiceResult<Evaluation>> AjouterEvaluationAsync(int employeId, Evaluation evaluation);
        Task<ServiceResult<bool>> TransfertEmployeAsync(int employeId, int nouveauDepartementId, decimal nouveauSalaire);
        Task<int> CountAsync(string? search = null);
    }
}
