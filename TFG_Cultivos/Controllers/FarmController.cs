using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using TFG_Cultivos.Models;
using TFG_Cultivos.Services;
using TFG_Cultivos.Services.ExcelConversionService;
using static TFG_Cultivos.Models.GeminiResponseDTO;

namespace TFG_Cultivos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FarmController : ControllerBase
    {
        private readonly PacContext _context;
        private readonly IExcelConversionService _excelService;
        private readonly IConfiguration _config;
        private readonly string _apiKey;

        public FarmController(PacContext context, IExcelConversionService excelService, IConfiguration config)
        {
            _context = context;
            _excelService = excelService;
            _config = config;
            _apiKey = _config["apiKeyGemini"];
        }

        [Route("getAll")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var parcelas = _context.Parcelas.ToList();
            return Ok(parcelas);
        }


        [HttpPost("importar-pac")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportarPacDesdeExcel([FromForm] ImportPacRequest request)
        {
            if (request.ArchivoExcel == null || request.ArchivoExcel.Length == 0)
                return BadRequest("No se ha enviado ningún archivo.");

            string usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int añoCampaña = request.Anio;

            var errores = new List<string>();

            var workbook = _excelService.ConvertToXlsx(request.ArchivoExcel);
            var ws = ObtenerHojaParcelas(workbook);

            int fila = 14;

            while (true)
            {
                var row = ws.Row(fila);

                if (row.Cell(2).IsEmpty())
                    break;

                try
                {
                    // Validaciones
                    if (!row.Cell(7).TryGetValue<int>(out int poligono))
                    {
                        fila++;
                        continue;
                    }

                    if (!row.Cell(8).TryGetValue<int>(out int parcelaNum))
                    {
                        fila++;
                        continue;
                    }

                    // PARCELA
                    var parcela = await _context.Parcelas
                        .FirstOrDefaultAsync(p =>
                            p.UsuarioId == usuarioId &&
                            p.Poligono == poligono &&
                            p.ParcelaNumero == parcelaNum);

                    if (parcela == null)
                    {
                        parcela = new Parcelas
                        {
                            UsuarioId = usuarioId,
                            CodigoProvincia = row.Cell(3).GetString().Trim(),
                            Municipio = row.Cell(4).GetString().Trim(),
                            CodigoAgregado = row.Cell(5).GetString().Trim(),
                            Zona = row.Cell(6).GetString().Trim(),
                            Poligono = poligono,
                            ParcelaNumero = parcelaNum
                        };

                        _context.Parcelas.Add(parcela);
                        await _context.SaveChangesAsync();
                    }

                    // RECINTO
                    if (!row.Cell(9).TryGetValue<int>(out int recintoNum))
                    {
                        fila++;
                        continue;
                    }

                    var recinto = await _context.Recintos
                        .FirstOrDefaultAsync(r =>
                            r.ParcelaId == parcela.Id &&
                            r.IdRecinto == recintoNum);

                    if (recinto == null)
                    {
                        recinto = new Recintos
                        {
                            Parcela = parcela,
                            IdRecinto = recintoNum,
                            UsoSigpac = row.Cell(10).GetString().Trim(),
                            SuperficieSigpac = row.Cell(11).GetValue<decimal>()
                        };

                        _context.Recintos.Add(recinto);
                        await _context.SaveChangesAsync();
                    }

                    //DATOS AGRONÓMICOS(HISTÓRICO)
                    bool existeDato = await _context.DatoAgronomico.AnyAsync(d =>
                         d.RecintoId == recinto.Id &&
                         d.AñoCampaña == añoCampaña);

                    if (!existeDato)
                    {
                        var dato = new DatoAgronomico
                        {
                            Recinto = recinto,
                            AñoCampaña = añoCampaña,
                            SuperficieCultivada = row.Cell(12).GetValue<decimal>(),
                            EspecieVariedad = row.Cell(13).GetString().Trim(),
                            EcoregimenPractica = row.Cell(14).GetString().Trim(),
                            SecanoRegadio = row.Cell(15).GetString().Trim(),
                            CultivoPrincipalSecundario = row.Cell(16).GetString().Trim(),
                            FechaInicio = row.Cell(17).TryGetValue(out DateTime fi) ? fi : null,
                            FechaFin = row.Cell(18).TryGetValue(out DateTime ff) ? ff : null,
                            AireLibreProtegido = row.Cell(19).GetString().Trim()
                        };

                        _context.DatoAgronomico.Add(dato);
                    }
                }

                catch (Exception ex)
                {
                    errores.Add($"Fila {fila}: {ex.Message}");
                }

                fila++;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Importación PAC finalizada correctamente",
                FilasProcesadas = fila - 2,
                Errores = errores
            });
        }

        private IXLWorksheet? ObtenerHojaParcelas(XLWorkbook workbook)
        {
            foreach (var sheet in workbook.Worksheets)
            {
                // Busca textos típicos PAC en las primeras filas
                for (int fila = 1; fila <= 20; fila++)
                {
                    var textoFila = sheet.Row(fila)
                        .CellsUsed()
                        .Select(c => c.GetString().ToUpperInvariant())
                        .ToList();

                    if (textoFila.Any(t => t.Contains("2.1 DATOS IDENTIFICATIVOS Y AGRONÓMICOS DE LAS PARCELAS"))
                        )
                    {
                        return sheet;
                    }
                }
            }

            return null;
        }

        [HttpPost("generar-propuesta-ia")]
        public async Task<IActionResult> GenerarPropuestaIa(GenerarPropuestaRequest elecciones)
        {
            string usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int[] campanias = { elecciones.AnioCampania - 1, elecciones.AnioCampania - 2, elecciones.AnioCampania - 3 };

            if (elecciones.CultivosPermitidos == null || !elecciones.CultivosPermitidos.Any())
                return BadRequest("Debe seleccionar al menos un cultivo permitido.");

            // 1️ Cargar explotación
            var recintos = await _context.Recintos
                .Include(r => r.Parcela)
                .Include(r => r.DatosAgronomicos)
                .Where(r => r.Parcela.UsuarioId == usuarioId)
                .ToListAsync();

            if (!recintos.Any())
                return BadRequest("El usuario no tiene recintos cargados.");

            // 2️ Construir JSON para IA
            var recintosIa = recintos.Select(r => new
            {
                recintoId = r.Id,
                superficie = r.SuperficieSigpac,
                historial = r.DatosAgronomicos
                    .Where(d => campanias.Contains(d.AñoCampaña))
                    .OrderByDescending(d => d.AñoCampaña)
                    .Select(d => new
                    {
                        anio = d.AñoCampaña,
                        cultivo = d.EspecieVariedad
                    })
                    .ToList()
            });

            var superficieTotal = recintos.Sum(r => r.SuperficieSigpac);

            var payloadIa = new
            {
                campaniaObjetivo = elecciones.AnioCampania,
                cultivosPermitidos = elecciones.CultivosPermitidos,
                ecorregimenesSolicitados = elecciones.EcorregimenesObjetivo,
                superficieTotal,
                criteriosPAC = new
                {
                    rotacion = true,
                    diversificacion = true,
                    leguminosasMin = 10
                },
                recintos = recintosIa
            };
            string payloadJson = JsonSerializer.Serialize(payloadIa);

            // 3️ Llamada única a Gemini
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(180);
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={_apiKey}");
            string systemInstructions = Constants.systemInstructions;
            var parts = new List<object>();

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            var requestBody = new
            {
                contents = new[]
    {
        new
        {
            role = "user",
            parts = new[]
            {
                new { text = $"INSTRUCCIONES CLAVE:\n{Constants.systemInstructions2}" },
                new { text = $"DATOS DE LA EXPLOTACIÓN:\n{payloadJson}" }
            }
        }
    },
                generationConfig = new
                {
                    temperature = 0.1

                }
            };
            // 2. Serializamos de forma que C# NO toque las minúsculas/mayúsculas
            var options = new JsonSerializerOptions { PropertyNamingPolicy = null };
            string finalJson = JsonSerializer.Serialize(requestBody, options);

            var content = new StringContent(finalJson, Encoding.UTF8, "application/json");

            request.Content = content;
            HttpResponseMessage response;

            try
            {
                response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorBody);
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error llamando a IA: {ex.Message}");
            }
            var rawJson = await response.Content.ReadAsStringAsync();

            // 4️ Procesar respuesta IA
            PropuestaIaDto respuestaIa;

            try
            {
                var jsonResponse = JsonNode.Parse(rawJson);
                string text = jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.GetValue<string>();

                string cleanedJson = text
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                respuestaIa = JsonSerializer.Deserialize<PropuestaIaDto>(cleanedJson)!;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error parseando IA: {ex.Message}");
            }

            // 5️ Guardar BORRADOR
            foreach (var r in respuestaIa.Asignaciones)
            {
                if (!int.TryParse(r.ParcelaId, out int recintoId))
                    continue;
                _context.PropuestasCultivo.Add(new PropuestaCultivo
                {
                    UsuarioId = usuarioId,
                    RecintoId = recintoId,
                    AnioCampania = elecciones.AnioCampania,
                    CultivoPropuesto = r.CultivoRecomendado,
                    Justificacion = JsonSerializer.Serialize(new
                    {
                        pac = r.JustificacionPac,
                        agronomico = r.BeneficioAgronomico
                    }),
                    EsBorrador = true
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Propuesta IA generada y guardada como borrador",
                anio = elecciones.AnioCampania,
                cumplePac = respuestaIa.ResumenExplotacion.CumplePac,
                porcentajeMejorantes = respuestaIa.ResumenExplotacion.PorcentajeMejorantes,
                totalRecintos = respuestaIa.Asignaciones.Count
            });
        }

        [HttpGet("exportar-propuesta")]
        public async Task<IActionResult> ExportarPropuestaExcel(
    int anio,
    bool soloBorrador = true)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var propuestas = await _context.PropuestasCultivo
                .Include(p => p.Recinto)
                    .ThenInclude(r => r.Parcela)
                .Where(p => p.UsuarioId == usuarioId &&
                            p.AnioCampania == anio)
                .ToListAsync();

            if (!propuestas.Any())
                return BadRequest("No hay propuestas para exportar.");

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Propuesta PAC");

            // CABECERAS
            ws.Cell(1, 1).Value = "Municipio";
            ws.Cell(1, 2).Value = "Polígono";
            ws.Cell(1, 3).Value = "Parcela";
            ws.Cell(1, 4).Value = "Superficie (ha)";
            ws.Cell(1, 5).Value = "Cultivo propuesto";
            ws.Cell(1, 6).Value = "Justificación";

            int fila = 2;

            foreach (var p in propuestas)
            {
                // Limpieza de JSON
                string justificacionLimpia= LimpiarJustificacion(p.Justificacion);

                ws.Cell(fila, 1).Value = p.Recinto.Parcela.Municipio;
                ws.Cell(fila, 2).Value = p.Recinto.Parcela.Poligono;
                ws.Cell(fila, 3).Value = p.Recinto.Parcela.ParcelaNumero;
                ws.Cell(fila, 4).Value = p.Recinto.SuperficieSigpac;
                ws.Cell(fila, 5).Value = p.CultivoPropuesto;
                ws.Cell(fila, 6).Value = justificacionLimpia;

                fila++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"propuesta_pac_{anio}.xlsx"
            );
        }

        [HttpGet("ver-en-sigpac")]
        public async Task<IActionResult> VerEnSigpac(int id)
        {
            var parcela = await _context.Parcelas
                .FirstOrDefaultAsync(x => x.Id == id);

            var queryParams = new Dictionary<string, string?>
            {
                ["provincia"] = parcela.CodigoProvincia,
                ["municipio"] = parcela.Municipio,
                ["agregado"] = parcela.CodigoAgregado ?? "0",
                ["zona"] = parcela.Zona ?? "0",
                ["poligono"] = parcela.Poligono.ToString(),
                ["parcela"] = parcela.ParcelaNumero.ToString(),
            };

            var queryString = string.Join("&",
                queryParams
                    .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                    .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}")
            );

            var urlFinal = $"{Constants.SIGPAC_VISOR_URL}/?{queryString}";

            return Ok(new { visorUrl = urlFinal });
        }

        [HttpPut("asignar-nombre")]
        public async Task<IActionResult> AsignarNombreParcela(int parcelaId, string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest("El nombre no puede estar vacío.");

            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var parcela = await _context.Parcelas
                .FirstOrDefaultAsync(p => p.Id == parcelaId && p.UsuarioId == usuarioId);

            if (parcela == null)
                return NotFound("Parcela no encontrada.");

            parcela.NombrePersonalizado = nombre;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Nombre de parcela actualizado",
                ParcelaId = parcelaId,
                Nombre = parcela.NombrePersonalizado
            });
        }

        private static string LimpiarJustificacion(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return string.Empty;

            try
            {
                // Caso A: string JSON
                if (raw.TrimStart().StartsWith("\""))
                    return JsonSerializer.Deserialize<string>(raw);

                // Caso B: objeto JSON
                if (raw.TrimStart().StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(raw);
                    return string.Join(" | ",
                        doc.RootElement.EnumerateObject()
                            .Select(p => p.Value.GetString())
                            .Where(v => !string.IsNullOrWhiteSpace(v))
                    );
                }
            }
            catch
            {
            }
            return raw;
        }
    }
}
