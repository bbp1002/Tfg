using System;

namespace TFG_Cultivos.Models
{
    public class DatoAgronomico
    {
        public int IdDato { get; set; }
        public int IdRecinto { get; set; }
        public int IdCampania { get; set; }

        public string EspecieVariedad { get; set; }
        public string EcoregimenPractica { get; set; }
        public string SecanoRegadio { get; set; }
        public string CultivoPrincipalSecundario { get; set; }
        public DateTime? FechaInicioCultivo { get; set; }
        public DateTime? FechaFinCultivo { get; set; }
        public string AireLibreProtegido { get; set; }

        public RecintoSigpac Recinto { get; set; }
        public Campania Campania { get; set; }
    }
}
