namespace TFG_Cultivos.Models
{
    public class ParcelaDto
    {
        public int ParcelaId { get; set; }
        public string Nombre { get; set; }

        public string Provincia { get; set; }
        public string Municipio { get; set; }
        public int Poligono { get; set; }
        public int NumeroParcela { get; set; }

        public decimal SuperficieTotal { get; set; }

        public List<RecintoDto> Recintos { get; set; }
    }

}
