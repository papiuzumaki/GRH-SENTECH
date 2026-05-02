using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GRH_SENTECH.Models.Enums;

namespace GRH_SENTECH.Models
{
    [Table("contrats")]
    public class Contrat
    {
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le type de contrat est obligatoire")]
        [Column("type_contrat")]
        public TypeContrat TypeContrat { get; set; }

        [Required(ErrorMessage = "La date de début est obligatoire")]
        [Column("date_debut")]
        public DateTime DateDebut { get; set; }

        // Nullable si CDI
        [Column("date_fin")]
        public DateTime? DateFin { get; set; }

        [Required(ErrorMessage = "Le salaire de base est obligatoire")]
        [Range(0, double.MaxValue, ErrorMessage = "Le salaire doit être positif")]
        [Column("salaire_base")]
        public decimal SalaireBase { get; set; }

        [Column("periode_essai")]
        public bool PeriodeEssai { get; set; }

        [Column("employe_id")]
        public int EmployeId { get; set; }

        // Navigation
        public Employe? Employe { get; set; }
    }
}
