using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GRH_SENTECH.Models
{
    [Table("departements")]
    public class Departement
    {
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Le nom doit avoir entre 3 et 50 caractères")]
        [Column("nom")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le code est obligatoire")]
        [StringLength(6, MinimumLength = 3, ErrorMessage = "Le code doit avoir entre 3 et 6 caractères")]
        [Column("code")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le budget est obligatoire")]
        [Range(0, double.MaxValue, ErrorMessage = "Le budget doit être positif")]
        [Column("budget")]
        public decimal Budget { get; set; }

        // Navigation
        public ICollection<Employe> Employes { get; set; } = new List<Employe>();
    }
}
