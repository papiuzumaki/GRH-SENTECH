using System.ComponentModel.DataAnnotations;
using GRH_SENTECH.Models.Enums;

namespace GRH_SENTECH.ViewModels
{
    public class CongeViewModel
    {
        public int Id { get; set; }

        public int EmployeId { get; set; }
        public string? EmployeNomComplet { get; set; }

        [Required(ErrorMessage = "Le type de congé est obligatoire")]
        [Display(Name = "Type de congé")]
        public TypeConge TypeConge { get; set; }

        [Required(ErrorMessage = "La date de début est obligatoire")]
        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [Required(ErrorMessage = "La date de fin est obligatoire")]
        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        public DateTime DateFin { get; set; }

        [Display(Name = "Motif")]
        public string? Motif { get; set; }

        [Display(Name = "Statut")]
        public StatutConge Statut { get; set; }

        public int NbJours => DateFin > DateDebut ? (DateFin - DateDebut).Days + 1 : 0;
    }
}
