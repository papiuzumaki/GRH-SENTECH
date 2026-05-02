using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GRH_SENTECH.Data;
using GRH_SENTECH.Models;
using GRH_SENTECH.Models.Enums;

namespace GRH_SENTECH.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalEmployes = await _context.Employes.CountAsync();
            ViewBag.TotalDepartements = await _context.Departements.CountAsync();
            ViewBag.TotalPostes = await _context.Postes.CountAsync();
            ViewBag.CongesEnAttente = await _context.Conges
                .CountAsync(c => c.Statut == StatutConge.EnAttente);
            ViewBag.EmployesSansContrat = await _context.Employes
                .CountAsync(e => !e.Contrats.Any(c => c.DateFin == null || c.DateFin >= DateTime.UtcNow));

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
