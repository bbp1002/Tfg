using System.Collections.Generic;
using NetTopologySuite.Geometries; // si usas PostGIS para coordenadas

namespace TFG_Cultivos.Models
{
    public class RecintoSigpac
    {
        public int IdRecinto { get; set; }
        public int IdMunicipio { get; set; }
        public string CodigoAgregado { get; set; }
        public string Zona { get; set; }
        public string Poligono { get; set; }
        public string Parcela { get; set; }
        public string Recinto { get; set; }
        public string UsoSigpac { get; set; }
        public decimal? SuperficieSigpac { get; set; }
        public decimal? SuperficieCultivada { get; set; }
        public string NombrePersonalizado { get; set; }
        public Point Coordenadas { get; set; } // tipo geométrico (si no lo usas aún, coméntalo)
        public string SigpacUrl { get; set; }

        public Municipio Municipio { get; set; }
        public ICollection<DatoAgronomico> DatosAgronomicos { get; set; } = new List<DatoAgronomico>();
    }
}
