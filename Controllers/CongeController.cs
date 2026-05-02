using Microsoft.AspNetCore.Mvc;
using GRH_SENTECH.Filters;
using GRH_SENTECH.Models;
using GRH_SENTECH.Models.Enums;
using GRH_SENTECH.Services;
using GRH_SENTECH.ViewModels;

namespace GRH_SENTECH.Controllers
{
    [ServiceFilter(typeof(JournalisationActionFilter))]
    public class CongeController : Controller
    {
        private readonly ICongeService _congeService;
        private readonly IEmployeService _employeService;

        public CongeController(ICongeService congeService, IEmployeService employeService)
        {
            _congeService = congeService;
            _employeService = employeService;
        }

        // GET: Conge/Employe/5
        public async Task<IActionResult> ParEmploye(int id)
        {
            var empResult = await _employeService.GetByIdAsync(id);
            if (!empResult.Success)
            {
                TempData["Erreur"] = empResult.Message;
                return RedirectToAction("Index", "Employe");
            }

            var congeResult = await _congeService.GetByEmployeAsync(id);
            var employe = empResult.Data!;

            var vms = congeResult.Data?.Select(c => new CongeViewModel
            {
                Id = c.Id,
                EmployeId = c.EmployeId,
                EmployeNomComplet = $"{employe.Prenom} {employe.Nom}",
                TypeConge = c.TypeConge,
                DateDebut = c.DateDebut,
                DateFin = c.DateFin,
                Motif = c.Motif,
                Statut = c.Statut
            }) ?? Enumerable.Empty<CongeViewModel>();

            ViewBag.EmployeId = id;
            ViewBag.EmployeNomComplet = $"{employe.Prenom} {employe.Nom}";
            return View(vms);
        }

        // GET: Conge/Demander/5
        public async Task<IActionResult> Demander(int id)
        {
            var empResult = await _employeService.GetByIdAsync(id);
            if (!empResult.Success)
            {
                TempData["Erreur"] = empResult.Message;
                return RedirectToAction("Index", "Employe");
            }

            var employe = empResult.Data!;
            var vm = new CongeViewModel
            {
                EmployeId = id,
                EmployeNomComplet = $"{employe.Prenom} {employe.Nom}",
                DateDebut = DateTime.Today,
                DateFin = DateTime.Today.AddDays(7)
            };

            return View(vm);
        }

        // POST: Conge/Demander
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Demander(CongeViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var conge = new Conge
            {
                TypeConge = vm.TypeConge,
                DateDebut = DateTime.SpecifyKind(vm.DateDebut, DateTimeKind.Utc),
                DateFin = DateTime.SpecifyKind(vm.DateFin, DateTimeKind.Utc),
                Motif = vm.Motif,
                Statut = StatutConge.EnAttente
            };

            var result = await _congeService.DemanderCongeAsync(vm.EmployeId, conge);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(vm);
            }

            TempData["Succes"] = result.Message;
            return RedirectToAction(nameof(ParEmploye), new { id = vm.EmployeId });
        }

        // POST: Conge/Approuver/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approuver(int id, int employeId)
        {
            var result = await _congeService.ChangerStatutAsync(id, StatutConge.Approuve);
            TempData[result.Success ? "Succes" : "Erreur"] = result.Message;
            return RedirectToAction(nameof(ParEmploye), new { id = employeId });
        }

        // POST: Conge/Refuser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refuser(int id, int employeId)
        {
            var result = await _congeService.ChangerStatutAsync(id, StatutConge.Refuse);
            TempData[result.Success ? "Succes" : "Erreur"] = result.Message;
            return RedirectToAction(nameof(ParEmploye), new { id = employeId });
        }
    }
}
