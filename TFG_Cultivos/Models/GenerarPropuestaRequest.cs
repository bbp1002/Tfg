namespace TFG_Cultivos.Models
{
    public class GenerarPropuestaRequest
    {
        public int AnioCampania { get; set; }

        public List<string> EcorregimenesObjetivo { get; set; }

        public List<string> CultivosPermitidos { get; set; }
    }

}
