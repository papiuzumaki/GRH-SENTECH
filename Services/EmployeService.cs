using Microsoft.EntityFrameworkCore;
using GRH_SENTECH.Data;
using GRH_SENTECH.Models;
using GRH_SENTECH.Models.Enums;
using GRH_SENTECH.Repositories;

namespace GRH_SENTECH.Services
{
    public class EmployeService : IEmployeService
    {
        private readonly IEmployeRepository _employeRepo;
        private readonly IContratRepository _contratRepo;
        private readonly ApplicationDbContext _context;

        public EmployeService(IEmployeRepository employeRepo, IContratRepository contratRepo, ApplicationDbContext context)
        {
            _employeRepo = employeRepo;
            _contratRepo = contratRepo;
            _context = context;
        }

        public async Task<ServiceResult<IEnumerable<Employe>>> GetAllAsync(int page, int pageSize, string? search = null)
        {
            try
            {
                var employes = await _employeRepo.GetPagedAsync(page, pageSize, search);
                return ServiceResult<IEnumerable<Employe>>.Ok(employes);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Employe>>.Fail($"Erreur lors de la récupération : {ex.Message}");
            }
        }

        public async Task<ServiceResult<Employe>> GetByIdAsync(int id)
        {
            try
            {
                var employe = await _employeRepo.GetByIdWithDetailsAsync(id);
                if (employe == null)
                    return ServiceResult<Employe>.Fail("Employé introuvable.");
                return ServiceResult<Employe>.Ok(employe);
            }
            catch (Exception ex)
            {
                return ServiceResult<Employe>.Fail($"Erreur : {ex.Message}");
            }
        }

        public async Task<ServiceResult<Employe>> CreerEmployeAsync(Employe employe)
        {
            try
            {
                // RG05 : L'employé doit avoir au moins 18 ans
                var age = DateTime.Today.Year - employe.DateNaissance.Year;
                if (employe.DateNaissance.Date > DateTime.Today.AddYears(-age)) age--;
                if (age < 18)
                    return ServiceResult<Employe>.Fail("L'employé doit avoir au moins 18 ans à la date d'embauche. (RG05)");

                await _employeRepo.AddAsync(employe);
                await _employeRepo.SaveAsync();
                return ServiceResult<Employe>.Ok(employe, "Employé créé avec succès.");
            }
            catch (Exception ex)
            {
                return ServiceResult<Employe>.Fail($"Erreur lors de la création : {ex.Message}");
            }
        }

        public async Task<ServiceResult<Employe>> ModifierEmployeAsync(Employe employe)
        {
            try
            {
                var existant = await _employeRepo.GetByIdAsync(employe.Id);
                if (existant == null)
                    return ServiceResult<Employe>.Fail("Employé introuvable.");

                _employeRepo.Update(employe);
                await _employeRepo.SaveAsync();
                return ServiceResult<Employe>.Ok(employe, "Employé modifié avec succès.");
            }
            catch (Exception ex)
            {
                return ServiceResult<Employe>.Fail($"Erreur lors de la modification : {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> SupprimerEmployeAsync(int id)
        {
            try
            {
                var employe = await _employeRepo.GetByIdWithDetailsAsync(id);
                if (employe == null)
                    return ServiceResult<bool>.Fail("Employé introuvable.");

                // RG03 : Impossible si contrat CDI actif ou congés en attente
                var contratActif = await _contratRepo.GetContratActifAsync(id);
                if (contratActif != null && contratActif.TypeContrat == TypeContrat.CDI)
                    return ServiceResult<bool>.Fail("Impossible de supprimer un employé avec un contrat CDI actif. (RG03)");

                bool aCongeEnAttente = employe.Conges.Any(c => c.Statut == StatutConge.EnAttente);
                if (aCongeEnAttente)
                    return ServiceResult<bool>.Fail("Impossible de supprimer un employé ayant des congés en attente. (RG03)");

                _employeRepo.Delete(employe);
                await _employeRepo.SaveAsync();
                return ServiceResult<bool>.Ok(true, "Employé supprimé avec succès.");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail($"Erreur lors de la suppression : {ex.Message}");
            }
        }

        public async Task<ServiceResult<Contrat>> AjouterContratAsync(int employeId, Contrat contrat)
        {
            try
            {
                var employe = await _employeRepo.GetByIdWithDetailsAsync(employeId);
                if (employe == null)
                    return ServiceResult<Contrat>.Fail("Employé introuvable.");

                // RG01 : Un seul contrat actif simultanément
                var contratActif = await _contratRepo.GetContratActifAsync(employeId);
                if (contratActif != null)
                    return ServiceResult<Contrat>.Fail("Cet employé possède déjà un contrat actif. (RG01)");

                // RG02 : Le salaire doit être dans la fourchette du poste
                var poste = await _context.Postes.FindAsync(employe.PosteId);
                if (poste != null)
                {
                    if (contrat.SalaireBase < poste.SalaireMin || contrat.SalaireBase > poste.SalaireMax)
                        return ServiceResult<Contrat>.Fail($"Le salaire doit être compris entre {poste.SalaireMin} et {poste.SalaireMax} FCFA pour ce poste. (RG02)");
                }

                // RG07 : Le budget du département ne doit pas être dépassé
                var departement = await _context.Departements
                    .Include(d => d.Employes)
                    .ThenInclude(e => e.Contrats)
                    .FirstOrDefaultAsync(d => d.Id == employe.DepartementId);

                if (departement != null)
                {
                    decimal totalSalaires = departement.Employes
                        .SelectMany(e => e.Contrats)
                        .Where(c => c.DateFin == null || c.DateFin >= DateTime.Today)
                        .Sum(c => c.SalaireBase);

                    if (totalSalaires + contrat.SalaireBase > departement.Budget)
                        return ServiceResult<Contrat>.Fail("L'ajout de ce contrat dépasse le budget du département. (RG07)");
                }

                contrat.EmployeId = employeId;
                await _contratRepo.AddAsync(contrat);
                await _contratRepo.SaveAsync();
                return ServiceResult<Contrat>.Ok(contrat, "Contrat ajouté avec succès.");
            }
            catch (Exception ex)
            {
                return ServiceResult<Contrat>.Fail($"Erreur lors de l'ajout du contrat : {ex.Message}");
            }
        }

        public async Task<ServiceResult<Evaluation>> AjouterEvaluationAsync(int employeId, Evaluation evaluation)
        {
            try
            {
                var employe = await _employeRepo.GetByIdWithDetailsAsync(employeId);
                if (employe == null)
                    return ServiceResult<Evaluation>.Fail("Employé introuvable.");

                // RG06 : L'employé doit avoir au moins 6 mois d'ancienneté
                var premierContrat = employe.Contrats.OrderBy(c => c.DateDebut).FirstOrDefault();
                if (premierContrat == null)
                    return ServiceResult<Evaluation>.Fail("L'employé n'a aucun contrat enregistré.");

                var anciennete = (DateTime.Today - premierContrat.DateDebut).Days;
                if (anciennete < 180)
                    return ServiceResult<Evaluation>.Fail("L'employé doit avoir au moins 6 mois d'ancienneté pour être évalué. (RG06)");

                // RG04 : Note entre 0 et 20 (déjà géré par annotation, vérification explicite)
                if (evaluation.Note < 0 || evaluation.Note > 20)
                    return ServiceResult<Evaluation>.Fail("La note doit être comprise entre 0 et 20.");

                evaluation.EmployeId = employeId;
                await _context.Evaluations.AddAsync(evaluation);
                await _context.SaveChangesAsync();
                return ServiceResult<Evaluation>.Ok(evaluation, "Évaluation enregistrée avec succès.");
            }
            catch (Exception ex)
            {
                return ServiceResult<Evaluation>.Fail($"Erreur lors de l'ajout de l'évaluation : {ex.Message}");
            }
        }

        // RG08 (TransfertEmploye) — Exercice 2.3 : transaction explicite
        public async Task<ServiceResult<bool>> TransfertEmployeAsync(int employeId, int nouveauDepartementId, decimal nouveauSalaire)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var employe = await _employeRepo.GetByIdWithDetailsAsync(employeId);
                if (employe == null)
                    return ServiceResult<bool>.Fail("Employé introuvable.");

                var nouveauDept = await _context.Departements
                    .Include(d => d.Employes)
                    .ThenInclude(e => e.Contrats)
                    .FirstOrDefaultAsync(d => d.Id == nouveauDepartementId);

                if (nouveauDept == null)
                    return ServiceResult<bool>.Fail("Département de destination introuvable.");

                // Vérification du budget du nouveau département (RG07)
                decimal totalSalaires = nouveauDept.Employes
                    .Where(e => e.Id != employeId)
                    .SelectMany(e => e.Contrats)
                    .Where(c => c.DateFin == null || c.DateFin >= DateTime.Today)
                    .Sum(c => c.SalaireBase);

                if (totalSalaires + nouveauSalaire > nouveauDept.Budget)
                    return ServiceResult<bool>.Fail("Le transfert dépasse le budget du département de destination. (RG07)");

                // Clôture de l'ancien contrat
                var contratActif = await _contratRepo.GetContratActifAsync(employeId);
                if (contratActif != null)
                {
                    contratActif.DateFin = DateTime.Today;
                    _contratRepo.Update(contratActif);
                }

                // Création du nouveau contrat
                var nouveauContrat = new Contrat
                {
                    EmployeId = employeId,
                    TypeContrat = contratActif?.TypeContrat ?? Models.Enums.TypeContrat.CDI,
                    DateDebut = DateTime.Today,
                    SalaireBase = nouveauSalaire,
                    PeriodeEssai = false
                };
                await _contratRepo.AddAsync(nouveauContrat);

                // Changement de département
                employe.DepartementId = nouveauDepartementId;
                _employeRepo.Update(employe);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServiceResult<bool>.Ok(true, "Transfert effectué avec succès.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.Fail($"Erreur lors du transfert, opération annulée : {ex.Message}");
            }
        }

        public async Task<int> CountAsync(string? search = null)
        {
            return await _employeRepo.CountAsync(search);
        }
    }
}
