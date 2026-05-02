using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GRH_SENTECH.Data;
using GRH_SENTECH.Filters;
using GRH_SENTECH.Models;
using GRH_SENTECH.Models.Enums;
using GRH_SENTECH.Services;
using GRH_SENTECH.ViewModels;

namespace GRH_SENTECH.Controllers
{
    [ServiceFilter(typeof(JournalisationActionFilter))]
    public class EmployeController : Controller
    {
        private readonly IEmployeService _employeService;
        private readonly ApplicationDbContext _context;
        private const int PAGE_SIZE = 10;

        public EmployeController(IEmployeService employeService, ApplicationDbContext context)
        {
            _employeService = employeService;
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, string? search = null)
        {
            int total = await _employeService.CountAsync(search);
            int totalPages = (int)Math.Ceiling(total / (double)PAGE_SIZE);

            var result = await _employeService.GetAllAsync(page, PAGE_SIZE, search);
            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return View(new EmployeIndexViewModel());
            }

            var vm = new EmployeIndexViewModel
            {
                Employes = result.Data!.Select(e => FromEntity(e)).ToList(),
                PageActuelle = page,
                TotalPages = totalPages,
                TotalEmployes = total,
                SearchTerm = search,
                PageSize = PAGE_SIZE
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _employeService.GetByIdAsync(id);
            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return RedirectToAction("Index");
            }
            return View(FromEntity(result.Data!));
        }

        public async Task<IActionResult> Create()
        {
            await ChargerListes();
            return View(new EmployeViewModel { DateNaissance = DateTime.Today.AddYears(-25) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await ChargerListes();
                return View(vm);
            }

            var result = await _employeService.CreerEmployeAsync(ToEntity(vm));
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                await ChargerListes();
                return View(vm);
            }

            TempData["Succes"] = "Employé ajouté avec succès.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _employeService.GetByIdAsync(id);
            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return RedirectToAction("Index");
            }

            await ChargerListes();
            return View(FromEntity(result.Data!));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await ChargerListes();
                return View(vm);
            }

            var result = await _employeService.ModifierEmployeAsync(ToEntity(vm));
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                await ChargerListes();
                return View(vm);
            }

            TempData["Succes"] = "Employé modifié.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employeService.GetByIdAsync(id);
            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return RedirectToAction("Index");
            }
            return View(FromEntity(result.Data!));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _employeService.SupprimerEmployeAsync(id);
            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return RedirectToAction("Delete", new { id });
            }

            TempData["Succes"] = "Employé supprimé.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AjouterContrat(int id)
        {
            var result = await _employeService.GetByIdAsync(id);
            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return RedirectToAction("Index");
            }

            var emp = result.Data!;
            return View(new AjouterContratViewModel
            {
                EmployeId = id,
                EmployeNomComplet = $"{emp.Prenom} {emp.Nom}",
                DateDebut = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjouterContrat(AjouterContratViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var contrat = new Contrat
            {
                TypeContrat = vm.TypeContrat,
                DateDebut = DateTime.SpecifyKind(vm.DateDebut, DateTimeKind.Utc),
                DateFin = vm.DateFin.HasValue ? DateTime.SpecifyKind(vm.DateFin.Value, DateTimeKind.Utc) : null,
                SalaireBase = vm.SalaireBase,
                PeriodeEssai = vm.PeriodeEssai
            };

            var result = await _employeService.AjouterContratAsync(vm.EmployeId, contrat);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(vm);
            }

            TempData["Succes"] = "Contrat ajouté.";
            return RedirectToAction("Details", new { id = vm.EmployeId });
        }

        // mapping entite -> viewmodel
        private static EmployeViewModel FromEntity(Employe e)
        {
            var contratActif = e.Contrats
                .Where(c => c.DateFin == null || c.DateFin >= DateTime.Today)
                .OrderByDescending(c => c.DateDebut)
                .FirstOrDefault();

            return new EmployeViewModel
            {
                Id = e.Id,
                Matricule = e.Matricule,
                Nom = e.Nom,
                Prenom = e.Prenom,
                Email = e.Email,
                DateNaissance = e.DateNaissance,
                Genre = e.Genre,
                DepartementId = e.DepartementId,
                PosteId = e.PosteId,
                DepartementNom = e.Departement?.Nom,
                PosteIntitule = e.Poste?.Intitule,
                TypeContratActif = contratActif?.TypeContrat.ToString()
            };
        }

        // mapping viewmodel -> entite
        private static Employe ToEntity(EmployeViewModel vm)
        {
            return new Employe
            {
                Id = vm.Id,
                Matricule = vm.Matricule,
                Nom = vm.Nom,
                Prenom = vm.Prenom,
                Email = vm.Email,
                DateNaissance = DateTime.SpecifyKind(vm.DateNaissance, DateTimeKind.Utc),
                Genre = vm.Genre,
                DepartementId = vm.DepartementId,
                PosteId = vm.PosteId
            };
        }

        private async Task ChargerListes()
        {
            var depts = await _context.Departements.OrderBy(d => d.Nom).ToListAsync();
            var postes = await _context.Postes.OrderBy(p => p.Intitule).ToListAsync();

            ViewBag.Departements = new SelectList(depts, "Id", "Nom");
            ViewBag.Postes = new SelectList(postes, "Id", "Intitule");
            ViewBag.Genres = new SelectList(
                Enum.GetValues(typeof(Genre)).Cast<Genre>()
                    .Select(g => new { Value = (int)g, Text = g.ToString() }),
                "Value", "Text");
        }
    }
}
