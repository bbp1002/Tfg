namespace TFG_Cultivos.Models
{
    public class PropuestaCultivo
    {
        public int Id { get; set; }

        public string UsuarioId { get; set; }

        public int RecintoId { get; set; }
        public Recintos Recinto { get; set; }

        public int AnioCampania { get; set; }

        public string CultivoPropuesto { get; set; }

        public string Justificacion { get; set; }
        // JSON

        public bool EsBorrador { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }


}
