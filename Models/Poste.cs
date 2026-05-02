using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GRH_SENTECH.Models
{
    [Table("postes")]
    public class Poste
    {
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "L'intitulé est obligatoire")]
        [Column("intitule")]
        public string Intitule { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le niveau hiérarchique est obligatoire")]
        [Range(1, 5, ErrorMessage = "Le niveau doit être compris entre 1 et 5")]
        [Column("niveau_hierarchique")]
        public int NiveauHierarchique { get; set; }

        [Required(ErrorMessage = "Le salaire minimum est obligatoire")]
        [Range(0, double.MaxValue, ErrorMessage = "Le salaire minimum doit être positif")]
        [Column("salaire_min")]
        public decimal SalaireMin { get; set; }

        [Required(ErrorMessage = "Le salaire maximum est obligatoire")]
        [Range(0, double.MaxValue, ErrorMessage = "Le salaire maximum doit être positif")]
        [Column("salaire_max")]
        public decimal SalaireMax { get; set; }

        // Navigation
        public ICollection<Employe> Employes { get; set; } = new List<Employe>();
    }
}
