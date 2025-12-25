using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFG_Cultivos.Models
{
    [Table("parcelas")]
    public class Parcelas
    {
        [Key]
        public int Id { get; set; }

        // Multiusuario
        public int UsuarioId { get; set; }

        // SIGPAC
        [MaxLength(10)]
        public string CodigoProvincia { get; set; }

        [MaxLength(100)]
        public string Municipio { get; set; }

        [MaxLength(10)]
        public string CodigoAgregado { get; set; }

        [MaxLength(10)]
        public string Zona { get; set; }

        public int Poligono { get; set; }
        public int ParcelaNumero { get; set; }

        // Navegación
        public ICollection<Recintos> Recintos { get; set; } = new List<Recintos>();
    }
}
