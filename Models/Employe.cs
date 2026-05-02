using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GRH_SENTECH.Models.Enums;

namespace GRH_SENTECH.Models
{
    [Table("employes")]
    public class Employe
    {
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le matricule est obligatoire")]
        [Column("matricule")]
        public string Matricule { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [Column("nom")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [Column("prenom")]
        public string Prenom { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La date de naissance est obligatoire")]
        [Column("date_naissance")]
        public DateTime DateNaissance { get; set; }

        [Required(ErrorMessage = "Le genre est obligatoire")]
        [Column("genre")]
        public Genre Genre { get; set; }

        [Column("departement_id")]
        public int DepartementId { get; set; }

        [Column("poste_id")]
        public int PosteId { get; set; }

        // Navigation
        public Departement? Departement { get; set; }
        public Poste? Poste { get; set; }
        public ICollection<Contrat> Contrats { get; set; } = new List<Contrat>();
        public ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
        public ICollection<Conge> Conges { get; set; } = new List<Conge>();
    }
}
