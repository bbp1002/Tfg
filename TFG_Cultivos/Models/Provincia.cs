using System.Collections.Generic;

namespace TFG_Cultivos.Models
{
    public class Provincia
    {
        public string IdProvincia { get; set; }  // CHAR(2)
        public string Nombre { get; set; }

        public ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
    }
}
