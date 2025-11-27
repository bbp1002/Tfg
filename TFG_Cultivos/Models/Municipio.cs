using System.Collections.Generic;

namespace TFG_Cultivos.Models
{
    public class Municipio
    {
        public int IdMunicipio { get; set; }
        public string IdProvincia { get; set; }
        public string Nombre { get; set; }

        public Provincia Provincia { get; set; }
        public ICollection<RecintoSigpac> Recintos { get; set; } = new List<RecintoSigpac>();
    }
}
