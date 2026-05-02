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
        private const int PageSize = 10;

        public EmployeController(IEmployeService employeService, ApplicationDbContext context)
        {
            _employeService = employeService;
            _context = context;
        }

        // GET: Employe
        public async Task<IActionResult> Index(int page = 1, string? search = null)
        {
            var totalEmployes = await _employeService.CountAsync(search);
            var totalPages = (int)Math.Ceiling(totalEmployes / (double)PageSize);

            var result = await _employeService.GetAllAsync(page, PageSize, search);
            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return View(new EmployeIndexViewModel());
            }

            var employes = result.Data!.Select(e => FromEntity(e)).ToList();

            var vm = new EmployeIndexViewModel
            {
                Employes = employes,
                PageActuelle = page,
                TotalPages = totalPages,
                TotalEmployes = totalEmployes,
                SearchTerm = search,
                PageSize = PageSize
            };

            return View(vm);
        }

        // GET: Employe/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _employeService.GetByIdAsync(id);
            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(FromEntity(result.Data!));
        }

        // GET: Employe/Create
        public async Task<IActionResult> Create()
        {
            await ChargerListesDeroulantes();
            return View(new EmployeViewModel { DateNaissance = DateTime.Today.AddYears(-25) });
        }

        // POST: Employe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await ChargerListesDeroulantes();
                return View(vm);
            }

            var employe = ToEntity(vm);
            var result = await _employeService.CreerEmployeAsync(employe);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await ChargerListesDeroulantes();
                return View(vm);
            }

            TempData["Succes"] = "Employé créé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Employe/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _employeService.GetByIdAsync(id);
            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            await ChargerListesDeroulantes();
            return View(FromEntity(result.Data!));
        }

        // POST: Employe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeViewModel vm)
        {
            if (id != vm.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await ChargerListesDeroulantes();
                return View(vm);
            }

            var employe = ToEntity(vm);
            var result = await _employeService.ModifierEmployeAsync(employe);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                await ChargerListesDeroulantes();
                return View(vm);
            }

            TempData["Succes"] = "Employé modifié avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Employe/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employeService.GetByIdAsync(id);
            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(FromEntity(result.Data!));
        }

        // POST: Employe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _employeService.SupprimerEmployeAsync(id);

            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }

            TempData["Succes"] = "Employé supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Employe/AjouterContrat/5
        public async Task<IActionResult> AjouterContrat(int id)
        {
            var result = await _employeService.GetByIdAsync(id);
            if (!result.Success)
            {
                TempData["Erreur"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var employe = result.Data!;
            var vm = new AjouterContratViewModel
            {
                EmployeId = id,
                EmployeNomComplet = $"{employe.Prenom} {employe.Nom}",
                DateDebut = DateTime.Today
            };

            return View(vm);
        }

        // POST: Employe/AjouterContrat
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
                ModelState.AddModelError(string.Empty, result.Message);
                return View(vm);
            }

            TempData["Succes"] = "Contrat ajouté avec succès.";
            return RedirectToAction(nameof(Details), new { id = vm.EmployeId });
        }

        // --- Helpers mapping ---

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
                TypeContratActif = contratActif != null ? contratActif.TypeContrat.ToString() : null
            };
        }

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

        private async Task ChargerListesDeroulantes()
        {
            var departements = await _context.Departements.OrderBy(d => d.Nom).ToListAsync();
            var postes = await _context.Postes.OrderBy(p => p.Intitule).ToListAsync();

            ViewBag.Departements = new SelectList(departements, "Id", "Nom");
            ViewBag.Postes = new SelectList(postes, "Id", "Intitule");
            ViewBag.Genres = new SelectList(Enum.GetValues(typeof(Genre)).Cast<Genre>()
                .Select(g => new { Value = (int)g, Text = g.ToString() }), "Value", "Text");
        }
    }
}
