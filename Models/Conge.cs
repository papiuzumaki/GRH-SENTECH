using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GRH_SENTECH.Models.Enums;

namespace GRH_SENTECH.Models
{
    [Table("conges")]
    public class Conge
    {
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le type de congé est obligatoire")]
        [Column("type_conge")]
        public TypeConge TypeConge { get; set; }

        [Required(ErrorMessage = "La date de début est obligatoire")]
        [Column("date_debut")]
        public DateTime DateDebut { get; set; }

        [Required(ErrorMessage = "La date de fin est obligatoire")]
        [Column("date_fin")]
        public DateTime DateFin { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire")]
        [Column("statut")]
        public StatutConge Statut { get; set; }

        [Column("motif")]
        public string? Motif { get; set; }

        [Column("employe_id")]
        public int EmployeId { get; set; }

        // Navigation
        public Employe? Employe { get; set; }
    }
}
