namespace TFG_Cultivos.Models
{
    public class Parcelas
    {
        public int Id { get; set; }
        public int Provincia { get; set; }
        public string TerminoMunicipal { get; set; }
        public int Agregado { get; set; }
        public int Zona { get; set; }
        public int Poligono { get; set; }
        public int Parcela { get; set; }
        public int Recinto { get; set; }
        public string UsoSIGPAC { get; set; }
        public double Superficie { get; set; }
        public double SuperficieCultivada { get; set; }
        public bool Secano { get; set; }
    }
}
