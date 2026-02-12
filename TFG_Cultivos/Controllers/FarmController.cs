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
    [Authorize] // Todos los endpoints requieren autenticación
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

        // --------------------------------------------------------------------
        // GET api/farm/getAll
        // Devuelve todas las parcelas del usuario (sin recintos ni histórico)
        // --------------------------------------------------------------------
        [Route("getAll")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var parcelas = _context.Parcelas.ToList();
            return Ok(parcelas);
        }

        // --------------------------------------------------------------------
        // POST api/farm/importar-pac
        // Importa un archivo PAC en Excel, detecta parcelas, recintos y datos
        // agronómicos, y los guarda en la base de datos.
        // --------------------------------------------------------------------
        [HttpPost("importar-pac")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportarPacDesdeExcel([FromForm] ImportPacRequest request)
        {
            if (request.ArchivoExcel == null || request.ArchivoExcel.Length == 0)
                return BadRequest("No se ha enviado ningún archivo.");

            string usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int añoCampaña = request.Anio;

            var errores = new List<string>();

            // Convierte el archivo a XLSX si viene en otro formato
            var workbook = _excelService.ConvertToXlsx(request.ArchivoExcel);

            // Busca la hoja correcta dentro del Excel PAC
            var ws = ObtenerHojaParcelas(workbook);

            int fila = 14; // La PAC suele empezar en esta fila

            while (true)
            {
                var row = ws.Row(fila);

                // Si la fila está vacía, se asume fin de datos
                if (row.Cell(2).IsEmpty())
                    break;

                try
                {
                    // Validaciones básicas de polígono y parcela
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

                    // -------------------------
                    // PARCELA
                    // -------------------------
                    var parcela = await _context.Parcelas
                        .FirstOrDefaultAsync(p =>
                            p.UsuarioId == usuarioId &&
                            p.Poligono == poligono &&
                            p.ParcelaNumero == parcelaNum);

                    // Si no existe, se crea
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

                    // -------------------------
                    // RECINTO
                    // -------------------------
                    if (!row.Cell(9).TryGetValue<int>(out int recintoNum))
                    {
                        fila++;
                        continue;
                    }

                    var recinto = await _context.Recintos
                        .FirstOrDefaultAsync(r =>
                            r.ParcelaId == parcela.Id &&
                            r.IdRecinto == recintoNum);

                    // Si no existe, se crea
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

                    // -------------------------
                    // DATOS AGRONÓMICOS (HISTÓRICO)
                    // -------------------------
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

        // --------------------------------------------------------------------
        // Método auxiliar que detecta la hoja correcta del Excel PAC
        // --------------------------------------------------------------------
        private IXLWorksheet? ObtenerHojaParcelas(XLWorkbook workbook)
        {
            foreach (var sheet in workbook.Worksheets)
            {
                for (int fila = 1; fila <= 20; fila++)
                {
                    var textoFila = sheet.Row(fila)
                        .CellsUsed()
                        .Select(c => c.GetString().ToUpperInvariant())
                        .ToList();

                    if (textoFila.Any(t => t.Contains("2.1 DATOS IDENTIFICATIVOS Y AGRONÓMICOS DE LAS PARCELAS")))
                    {
                        return sheet;
                    }
                }
            }

            return null;
        }

        // --------------------------------------------------------------------
        // POST api/farm/generar-propuesta-ia
        // Llama a Gemini con los datos de la explotación y genera una propuesta
        // de cultivos para la campaña seleccionada.
        // --------------------------------------------------------------------
        [HttpPost("generar-propuesta-ia")]
        public async Task<IActionResult> GenerarPropuestaIa(GenerarPropuestaRequest elecciones)
        {
            string usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Se usan los 3 años anteriores para el histórico
            int[] campanias = { elecciones.AnioCampania - 1, elecciones.AnioCampania - 2, elecciones.AnioCampania - 3 };

            if (elecciones.CultivosPermitidos == null || !elecciones.CultivosPermitidos.Any())
                return BadRequest("Debe seleccionar al menos un cultivo permitido.");

            // 1. Cargar recintos del usuario
            var recintos = await _context.Recintos
                .Include(r => r.Parcela)
                .Include(r => r.DatosAgronomicos)
                .Where(r => r.Parcela.UsuarioId == usuarioId)
                .ToListAsync();

            if (!recintos.Any())
                return BadRequest("El usuario no tiene recintos cargados.");

            // 2. Construir JSON para la IA
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

            // 3. Llamada a Gemini
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(270);
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

            var options = new JsonSerializerOptions { PropertyNamingPolicy = null };
            string finalJson = JsonSerializer.Serialize(requestBody, options);

            request.Content = new StringContent(finalJson, Encoding.UTF8, "application/json");

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

            // 4. Procesar respuesta de la IA
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

            // 5. Guardar propuesta como borrador
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

        // --------------------------------------------------------------------
        // GET api/farm/exportar-propuesta
        // Exporta a Excel la propuesta generada por la IA
        // --------------------------------------------------------------------
        [HttpGet("exportar-propuesta")]
        public async Task<IActionResult> ExportarPropuestaExcel(int anio, bool soloBorrador = true)
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

            // Cabeceras del Excel
            ws.Cell(1, 1).Value = "Municipio";
            ws.Cell(1, 2).Value = "Polígono";
            ws.Cell(1, 3).Value = "Parcela";
            ws.Cell(1, 4).Value = "Recinto";
            ws.Cell(1, 5).Value = "Superficie (ha)";
            ws.Cell(1, 6).Value = "Nombre";
            ws.Cell(1, 7).Value = "Cultivo propuesto";
            ws.Cell(1, 8).Value = "Justificación";

            int fila = 2;

            foreach (var p in propuestas)
            {
                string justificacionLimpia = LimpiarJustificacion(p.Justificacion);

                ws.Cell(fila, 1).Value = p.Recinto.Parcela.Municipio;
                ws.Cell(fila, 2).Value = p.Recinto.Parcela.Poligono;
                ws.Cell(fila, 3).Value = p.Recinto.Parcela.ParcelaNumero;
                ws.Cell(fila, 4).Value = p.Recinto.IdRecinto;
                ws.Cell(fila, 5).Value = p.Recinto.SuperficieSigpac;
                ws.Cell(fila, 6).Value = p.Recinto.Parcela.NombrePersonalizado;
                ws.Cell(fila, 7).Value = p.CultivoPropuesto;
                ws.Cell(fila, 8).Value = justificacionLimpia;

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

        // --------------------------------------------------------------------
        // GET api/farm/ver-en-sigpac
        // Devuelve la URL para abrir una parcela directamente en el visor SIGPAC
        // --------------------------------------------------------------------
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

        // --------------------------------------------------------------------
        // PUT api/farm/asignar-nombre
        // Permite asignar un nombre personalizado a una parcela
        // --------------------------------------------------------------------
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

        // --------------------------------------------------------------------
        // GET api/farm/con-historico
        // Devuelve todas las parcelas del usuario con sus recintos y el histórico
        // de cultivos de cada recinto.
        // --------------------------------------------------------------------
        [HttpGet("con-historico")]
        public async Task<IActionResult> GetParcelasConHistorico()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var parcelas = await _context.Parcelas
                .Where(p => p.UsuarioId == usuarioId)
                .Include(p => p.Recintos)
                    .ThenInclude(r => r.DatosAgronomicos)
                .ToListAsync();

            var result = parcelas.Select(p => new ParcelaDto
            {
                ParcelaId = p.Id,
                Nombre = p.NombrePersonalizado,

                Provincia = p.CodigoProvincia,
                Municipio = p.Municipio,
                Poligono = p.Poligono,
                NumeroParcela = p.ParcelaNumero,

                SuperficieTotal = p.Recintos.Sum(r => r.SuperficieSigpac),

                Recintos = p.Recintos.Select(r => new RecintoDto
                {
                    RecintoId = r.Id,
                    Superficie  = r.SuperficieSigpac,

                    Historico = r.DatosAgronomicos
                        .OrderByDescending(d => d.AñoCampaña)
                        .Select(d => new HistoricoCultivoDto
                        {
                            AnioCampania = d.AñoCampaña,
                            Cultivo = d.EspecieVariedad
                        })
                        .ToList()
                }).ToList()
            });

            return Ok(result);
        }

        // --------------------------------------------------------------------
        // GET api/farm/cultivos
        // Devuelve una lista fija de cultivos disponibles en la aplicación.
        // Esta lista se utiliza para mostrar opciones al usuario en el frontend
        // cuando debe seleccionar cultivos permitidos o asignar cultivos a recintos.
        // --------------------------------------------------------------------
        [HttpGet("cultivos")]
        public IActionResult GetCultivos()
        {
            var cultivos = new List<string>
        {
            "Trigo blando",
            "Trigo duro",
            "Cebada",
            "Avena",
            "Centeno",
            "Triticale",
            "Maíz",
            "Girasol",
            "Colza",
            "Barbecho",
            "Pastos permanentes",
            "Lentejas",
            "Guisantes",
            "Garbanzos",
            "Veza",
            "Yeros",
            "Patata",
            "Remolacha azucarera",
            "Alfalfa"
        };

            return Ok(cultivos);
        }

        // --------------------------------------------------------------------
        // GET api/farm/ecorregimenes
        // Devuelve una lista fija de ecorregímenes disponibles.
        // Se usa para que el usuario pueda seleccionar qué prácticas PAC
        // quiere solicitar en la campaña objetivo.
        // --------------------------------------------------------------------
        [HttpGet("ecorregimenes")]
        public IActionResult GetEcorregimenes()
        {
            var ecorregimenes = new List<string>
        {
            "BCAM-7 DIVERSIFICACION DE CULTIVOS",
            "BCAM-7 ROTACIÓN DE CULTIVOS",
            "P3: PRÁCTICA DE ROTACIÓN DE CULTIVOS CON ESPECIES MEJORANTES",
            "P4: PRÁCTICA DE LA SIEMBRA DIRECTA"
        };

            return Ok(ecorregimenes);
        }

        // --------------------------------------------------------------------
        // Método auxiliar: LimpiarJustificacion
        // Este método recibe la justificación almacenada en la base de datos,
        // que puede venir en distintos formatos (string JSON, objeto JSON, etc.).
        //
        // Su objetivo es devolver un texto limpio y legible para exportarlo a Excel.
        //
        // Casos contemplados:
        //  - Si es un string JSON (entre comillas), se deserializa y se devuelve.
        //  - Si es un objeto JSON, se extraen sus valores y se unen con " | ".
        //  - Si no se puede procesar, se devuelve el texto original.
        // --------------------------------------------------------------------
        private static string LimpiarJustificacion(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return string.Empty;

            try
            {
                // Caso A: el contenido es un string JSON (empieza con comillas)
                if (raw.TrimStart().StartsWith("\""))
                    return JsonSerializer.Deserialize<string>(raw);

                // Caso B: el contenido es un objeto JSON
                if (raw.TrimStart().StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(raw);

                    // Se concatenan los valores del objeto JSON en una sola línea
                    return string.Join(" | ",
                        doc.RootElement.EnumerateObject()
                            .Select(p => p.Value.GetString())
                            .Where(v => !string.IsNullOrWhiteSpace(v))
                    );
                }
            }
            catch
            {
                // Si algo falla, se devuelve el texto tal cual
            }

            return raw;
        }

    }
}
