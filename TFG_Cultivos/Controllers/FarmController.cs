using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TFG_Cultivos.Models;
using TFG_Cultivos.Services.ExcelConversionService;

namespace TFG_Cultivos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FarmController : ControllerBase
    {
        private readonly PacContext _context;
        private readonly IExcelConversionService _excelService;

        public FarmController(PacContext context, IExcelConversionService excelService)
        {
            _context = context;
            _excelService = excelService;
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

    }
}
