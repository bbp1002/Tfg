using System.Text.Json.Serialization;

namespace TFG_Cultivos.Models
{
    public class GeminiResponseDTO
    {
        public class PropuestaIaDto
        {
            [JsonPropertyName("resumen_explotacion")]
            public ResumenExplotacionDto ResumenExplotacion { get; set; }

            [JsonPropertyName("asignaciones")]
            public List<AsignacionParcelaDto> Asignaciones { get; set; }
        }


        public class ResumenExplotacionDto
        {
            [JsonPropertyName("cumple_pac")]
            public bool CumplePac { get; set; }

            [JsonPropertyName("porcentaje_mejorantes")]
            public decimal PorcentajeMejorantes { get; set; }
        }


        public class AsignacionParcelaDto
        {
            [JsonPropertyName("parcela_id")]
            public string ParcelaId { get; set; }

            [JsonPropertyName("cultivo_recomendado")]
            public string CultivoRecomendado { get; set; }

            [JsonPropertyName("justificacion_pac")]
            public string JustificacionPac { get; set; }

            [JsonPropertyName("beneficio_agronomico")]
            public string BeneficioAgronomico { get; set; }
        }

    }
}
