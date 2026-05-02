using GRH_SENTECH.Models;
using GRH_SENTECH.Models.Enums;
using GRH_SENTECH.Repositories;

namespace GRH_SENTECH.Services
{
    public class CongeService : ICongeService
    {
        private readonly ICongeRepository _congeRepo;
        private readonly IEmployeRepository _employeRepo;

        public CongeService(ICongeRepository congeRepo, IEmployeRepository employeRepo)
        {
            _congeRepo = congeRepo;
            _employeRepo = employeRepo;
        }

        public async Task<ServiceResult<IEnumerable<Conge>>> GetByEmployeAsync(int employeId)
        {
            try
            {
                var conges = await _congeRepo.GetByEmployeAsync(employeId);
                return ServiceResult<IEnumerable<Conge>>.Ok(conges);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Conge>>.Fail($"Erreur : {ex.Message}");
            }
        }

        public async Task<ServiceResult<Conge>> DemanderCongeAsync(int employeId, Conge conge)
        {
            try
            {
                var employe = await _employeRepo.GetByIdAsync(employeId);
                if (employe == null)
                    return ServiceResult<Conge>.Fail("Employé introuvable.");

                // DateFin > DateDebut
                if (conge.DateFin <= conge.DateDebut)
                    return ServiceResult<Conge>.Fail("La date de fin doit être postérieure à la date de début.");

                // RG04 : Max 30 jours de congés annuels par an
                if (conge.TypeConge == TypeConge.Annuel)
                {
                    int annee = conge.DateDebut.Year;
                    int joursDejaUtilises = await _congeRepo.GetTotalJoursAnnuelsAsync(employeId, annee);
                    int joursDemandes = (conge.DateFin - conge.DateDebut).Days + 1;

                    if (joursDejaUtilises + joursDemandes > 30)
                    {
                        int joursRestants = 30 - joursDejaUtilises;
                        return ServiceResult<Conge>.Fail(
                            $"Quota de congés annuels dépassé. Il vous reste {joursRestants} jour(s) pour {annee}. (RG04)");
                    }
                }

                conge.EmployeId = employeId;
                conge.Statut = StatutConge.EnAttente;
                await _congeRepo.AddAsync(conge);
                await _congeRepo.SaveAsync();

                return ServiceResult<Conge>.Ok(conge, "Demande de congé enregistrée avec succès.");
            }
            catch (Exception ex)
            {
                return ServiceResult<Conge>.Fail($"Erreur : {ex.Message}");
            }
        }

        public async Task<ServiceResult<Conge>> ChangerStatutAsync(int congeId, StatutConge nouveauStatut)
        {
            try
            {
                var conge = await _congeRepo.GetByIdAsync(congeId);
                if (conge == null)
                    return ServiceResult<Conge>.Fail("Congé introuvable.");

                conge.Statut = nouveauStatut;
                _congeRepo.Update(conge);
                await _congeRepo.SaveAsync();

                string msg = nouveauStatut == StatutConge.Approuve ? "Congé approuvé." : "Congé refusé.";
                return ServiceResult<Conge>.Ok(conge, msg);
            }
            catch (Exception ex)
            {
                return ServiceResult<Conge>.Fail($"Erreur : {ex.Message}");
            }
        }
    }
}
