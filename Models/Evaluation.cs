using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GRH_SENTECH.Models
{
    [Table("evaluations")]
    public class Evaluation
    {
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La période est obligatoire")]
        [Column("periode")]
        public string Periode { get; set; } = string.Empty;

        [Required(ErrorMessage = "La note est obligatoire")]
        [Range(0, 20, ErrorMessage = "La note doit être entre 0 et 20")]
        [Column("note")]
        public decimal Note { get; set; }

        [Column("commentaire")]
        public string? Commentaire { get; set; }

        [Required(ErrorMessage = "La date d'évaluation est obligatoire")]
        [Column("date_evaluation")]
        public DateTime DateEvaluation { get; set; }

        [Column("employe_id")]
        public int EmployeId { get; set; }

        // Navigation
        public Employe? Employe { get; set; }
    }
}
