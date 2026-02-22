namespace TFG_Cultivos.Models
{
    public class RecintoDto
    {
        public int RecintoId { get; set; }
        public decimal Superficie { get; set; }

        public List<HistoricoCultivoDto> Historico { get; set; }
    }

}
