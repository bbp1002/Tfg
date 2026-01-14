using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using NetTopologySuite.Geometries; // si usas PostGIS para coordenadas

namespace TFG_Cultivos.Models
{
    public class Recintos
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Parcelas))]
        public int ParcelaId { get; set; }

        public int IdRecinto { get; set; }

        [MaxLength(50)]
        public string UsoSigpac { get; set; }

        public decimal SuperficieSigpac { get; set; }

        // Navegación
        public Parcelas Parcela { get; set; }
        public ICollection<DatoAgronomico> DatosAgronomicos { get; set; } = new List<DatoAgronomico>();
    }
}
