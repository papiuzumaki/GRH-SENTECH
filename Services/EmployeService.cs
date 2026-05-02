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
            var employes = await _employeRepo.GetPagedAsync(page, pageSize, search);
            return ServiceResult<IEnumerable<Employe>>.Ok(employes);
        }

        public async Task<ServiceResult<Employe>> GetByIdAsync(int id)
        {
            var employe = await _employeRepo.GetByIdWithDetailsAsync(id);
            if (employe == null)
                return ServiceResult<Employe>.Fail("Employé introuvable.");
            return ServiceResult<Employe>.Ok(employe);
        }

        public async Task<ServiceResult<Employe>> CreerEmployeAsync(Employe employe)
        {
            // verifier que l'employe a au moins 18 ans (RG05)
            int age = DateTime.Today.Year - employe.DateNaissance.Year;
            if (employe.DateNaissance.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < 18)
                return ServiceResult<Employe>.Fail("L'employé doit avoir au moins 18 ans. (RG05)");

            await _employeRepo.AddAsync(employe);
            await _employeRepo.SaveAsync();
            return ServiceResult<Employe>.Ok(employe, "Employé créé avec succès.");
        }

        public async Task<ServiceResult<Employe>> ModifierEmployeAsync(Employe employe)
        {
            var emp = await _employeRepo.GetByIdAsync(employe.Id);
            if (emp == null)
                return ServiceResult<Employe>.Fail("Employé introuvable.");

            _employeRepo.Update(employe);
            await _employeRepo.SaveAsync();
            return ServiceResult<Employe>.Ok(employe, "Employé modifié.");
        }

        public async Task<ServiceResult<bool>> SupprimerEmployeAsync(int id)
        {
            var employe = await _employeRepo.GetByIdWithDetailsAsync(id);
            if (employe == null)
                return ServiceResult<bool>.Fail("Employé introuvable.");

            // RG03 : pas de suppression si CDI actif
            var contratActif = await _contratRepo.GetContratActifAsync(id);
            if (contratActif != null && contratActif.TypeContrat == TypeContrat.CDI)
                return ServiceResult<bool>.Fail("Impossible de supprimer un employé avec un contrat CDI actif.");

            // RG03 : pas de suppression si conge en attente
            if (employe.Conges.Any(c => c.Statut == StatutConge.EnAttente))
                return ServiceResult<bool>.Fail("Cet employé a des congés en attente, suppression impossible.");

            _employeRepo.Delete(employe);
            await _employeRepo.SaveAsync();
            return ServiceResult<bool>.Ok(true, "Employé supprimé.");
        }

        public async Task<ServiceResult<Contrat>> AjouterContratAsync(int employeId, Contrat contrat)
        {
            var employe = await _employeRepo.GetByIdWithDetailsAsync(employeId);
            if (employe == null)
                return ServiceResult<Contrat>.Fail("Employé introuvable.");

            // RG01 : un seul contrat actif a la fois
            var contratExistant = await _contratRepo.GetContratActifAsync(employeId);
            if (contratExistant != null)
                return ServiceResult<Contrat>.Fail("Cet employé a déjà un contrat actif.");

            // RG02 : salaire dans la fourchette du poste
            var poste = await _context.Postes.FindAsync(employe.PosteId);
            if (poste != null)
            {
                if (contrat.SalaireBase < poste.SalaireMin || contrat.SalaireBase > poste.SalaireMax)
                    return ServiceResult<Contrat>.Fail(
                        $"Le salaire doit être entre {poste.SalaireMin} et {poste.SalaireMax} FCFA pour ce poste.");
            }

            // RG07 : verifier que le budget du departement ne sera pas depasse
            var dept = await _context.Departements
                .Include(d => d.Employes)
                .ThenInclude(e => e.Contrats)
                .FirstOrDefaultAsync(d => d.Id == employe.DepartementId);

            if (dept != null)
            {
                decimal totalSalaires = dept.Employes
                    .SelectMany(e => e.Contrats)
                    .Where(c => c.DateFin == null || c.DateFin >= DateTime.Today)
                    .Sum(c => c.SalaireBase);

                if (totalSalaires + contrat.SalaireBase > dept.Budget)
                    return ServiceResult<Contrat>.Fail("Ce contrat dépasse le budget du département.");
            }

            contrat.EmployeId = employeId;
            await _contratRepo.AddAsync(contrat);
            await _contratRepo.SaveAsync();
            return ServiceResult<Contrat>.Ok(contrat, "Contrat ajouté.");
        }

        public async Task<ServiceResult<Evaluation>> AjouterEvaluationAsync(int employeId, Evaluation evaluation)
        {
            var employe = await _employeRepo.GetByIdWithDetailsAsync(employeId);
            if (employe == null)
                return ServiceResult<Evaluation>.Fail("Employé introuvable.");

            // RG06 : 6 mois d'anciennete minimum pour etre evalue
            var premierContrat = employe.Contrats.OrderBy(c => c.DateDebut).FirstOrDefault();
            if (premierContrat == null)
                return ServiceResult<Evaluation>.Fail("L'employé n'a pas de contrat enregistré.");

            int joursAnciennete = (DateTime.Today - premierContrat.DateDebut).Days;
            if (joursAnciennete < 180)
                return ServiceResult<Evaluation>.Fail("Il faut au moins 6 mois d'ancienneté pour créer une évaluation.");

            if (evaluation.Note < 0 || evaluation.Note > 20)
                return ServiceResult<Evaluation>.Fail("La note doit être entre 0 et 20.");

            evaluation.EmployeId = employeId;
            await _context.Evaluations.AddAsync(evaluation);
            await _context.SaveChangesAsync();
            return ServiceResult<Evaluation>.Ok(evaluation, "Évaluation enregistrée.");
        }

        // transfert d'un employe vers un autre departement avec transaction (Exercice 2.3)
        public async Task<ServiceResult<bool>> TransfertEmployeAsync(int employeId, int nouveauDeptId, decimal nouveauSalaire)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var employe = await _employeRepo.GetByIdWithDetailsAsync(employeId);
                if (employe == null)
                    return ServiceResult<bool>.Fail("Employé introuvable.");

                var nouveauDept = await _context.Departements
                    .Include(d => d.Employes).ThenInclude(e => e.Contrats)
                    .FirstOrDefaultAsync(d => d.Id == nouveauDeptId);

                if (nouveauDept == null)
                    return ServiceResult<bool>.Fail("Département introuvable.");

                // verifier le budget du nouveau departement
                decimal total = nouveauDept.Employes
                    .Where(e => e.Id != employeId)
                    .SelectMany(e => e.Contrats)
                    .Where(c => c.DateFin == null || c.DateFin >= DateTime.Today)
                    .Sum(c => c.SalaireBase);

                if (total + nouveauSalaire > nouveauDept.Budget)
                    return ServiceResult<bool>.Fail("Budget du département de destination insuffisant.");

                // on cloture l'ancien contrat
                var ancienContrat = await _contratRepo.GetContratActifAsync(employeId);
                if (ancienContrat != null)
                {
                    ancienContrat.DateFin = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
                    _contratRepo.Update(ancienContrat);
                }

                // nouveau contrat dans le nouveau departement
                var contrat = new Contrat
                {
                    EmployeId = employeId,
                    TypeContrat = ancienContrat?.TypeContrat ?? TypeContrat.CDI,
                    DateDebut = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc),
                    SalaireBase = nouveauSalaire,
                    PeriodeEssai = false
                };
                await _contratRepo.AddAsync(contrat);

                employe.DepartementId = nouveauDeptId;
                _employeRepo.Update(employe);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return ServiceResult<bool>.Ok(true, "Transfert effectué avec succès.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.Fail($"Erreur lors du transfert : {ex.Message}");
            }
        }

        public async Task<int> CountAsync(string? search = null)
        {
            return await _employeRepo.CountAsync(search);
        }
    }
}
