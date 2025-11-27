using System;
using System.Collections.Generic;

namespace TFG_Cultivos.Models
{
    public class Campania
    {
        public int IdCampania { get; set; }
        public int Anio { get; set; }
        public DateTime FechaImportacion { get; set; }

        public ICollection<DatoAgronomico> DatosAgronomicos { get; set; } = new List<DatoAgronomico>();
    }
}
