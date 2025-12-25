using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFG_Cultivos.Models
{
    public class DatoAgronomico
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Recintos))]
        public int RecintoId { get; set; }

        public int AñoCampaña { get; set; }

        public decimal SuperficieCultivada { get; set; }

        [MaxLength(100)]
        public string EspecieVariedad { get; set; }

        [MaxLength(100)]
        public string EcoregimenPractica { get; set; }

        [MaxLength(50)]
        public string SecanoRegadio { get; set; }

        [MaxLength(100)]
        public string CultivoPrincipalSecundario { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        [MaxLength(50)]
        public string AireLibreProtegido { get; set; }

        // Navegación
        public Recintos Recinto { get; set; }
    }
}
