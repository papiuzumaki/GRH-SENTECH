using System.ComponentModel.DataAnnotations;
using GRH_SENTECH.Models.Enums;

namespace GRH_SENTECH.ViewModels
{
    public class EmployeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le matricule est obligatoire")]
        [Display(Name = "Matricule")]
        public string Matricule { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [Display(Name = "Nom")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format email invalide")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de naissance est obligatoire")]
        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public DateTime DateNaissance { get; set; }

        [Required(ErrorMessage = "Le genre est obligatoire")]
        [Display(Name = "Genre")]
        public Genre Genre { get; set; }

        [Required(ErrorMessage = "Le département est obligatoire")]
        [Display(Name = "Département")]
        public int DepartementId { get; set; }

        [Required(ErrorMessage = "Le poste est obligatoire")]
        [Display(Name = "Poste")]
        public int PosteId { get; set; }

        // Affichage uniquement
        public string? DepartementNom { get; set; }
        public string? PosteIntitule { get; set; }
        public string? TypeContratActif { get; set; }
        public string NomComplet => $"{Prenom} {Nom}";
    }

    public class EmployeIndexViewModel
    {
        public IEnumerable<EmployeViewModel> Employes { get; set; } = new List<EmployeViewModel>();
        public int PageActuelle { get; set; }
        public int TotalPages { get; set; }
        public int TotalEmployes { get; set; }
        public string? SearchTerm { get; set; }
        public int PageSize { get; set; } = 10;
    }

    public class AjouterContratViewModel
    {
        public int EmployeId { get; set; }
        public string? EmployeNomComplet { get; set; }

        [Required(ErrorMessage = "Le type de contrat est obligatoire")]
        [Display(Name = "Type de contrat")]
        public TypeContrat TypeContrat { get; set; }

        [Required(ErrorMessage = "La date de début est obligatoire")]
        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }

        [Required(ErrorMessage = "Le salaire de base est obligatoire")]
        [Range(1, double.MaxValue, ErrorMessage = "Le salaire doit être positif")]
        [Display(Name = "Salaire de base (FCFA)")]
        public decimal SalaireBase { get; set; }

        [Display(Name = "Période d'essai")]
        public bool PeriodeEssai { get; set; }
    }
}
