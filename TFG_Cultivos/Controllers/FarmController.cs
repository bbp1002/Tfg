using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TFG_Cultivos.Models;

namespace TFG_Cultivos.Controllers
{
    public class FarmController : Controller
    {
        // GET: FarmController
        public ActionResult Index()
        {
            return View();
        }

        // GET: FarmController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FarmController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FarmController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FarmController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FarmController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FarmController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FarmController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpPost("PostImportarParcelasExcel")]
        public async Task<List<Parcelas>> PostImportarAcreedoresDesdeExcel(IFormFile archivoExcel)
        {
            var parcelas = new List<Parcelas>();
            var errores = new List<string>();

            using (var stream = new MemoryStream())
            {
                await archivoExcel.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
                    int fila = 2;

                    foreach (var row in rows)
                    {
                        try
                        {
                            string provinciaTexto = row.Cell(6).GetString().Trim();
                            string poblacionTexto = row.Cell(7).GetString().Trim();

                            int? provinciaId = await dataBase.Provincias
                                .Where(x => x.Nombre == provinciaTexto)
                                .Select(x => (int?)x.Id)
                                .FirstOrDefaultAsync();

                            int? poblacionId = await dataBase.Poblaciones
                                .Where(x => x.Nombre == poblacionTexto)
                                .Select(x => (int?)x.Id)
                                .FirstOrDefaultAsync();

                            if (provinciaId == null || poblacionId == null)
                            {
                                errores.Add($"Fila {fila}: Provincia o población no encontrada.");
                                fila++;
                                continue;
                            }

                            var parcela = new Parcelas
                            {
                                Nombre = row.Cell(1).GetString(),
                                NombreComercial = row.Cell(2).GetString(),
                                Nif = row.Cell(3).GetString(),
                                Direccion = row.Cell(4).GetString(),
                                CodigoPostal = row.Cell(5).GetString(),
                                Telefono = row.Cell(8).GetString(),
                                Email = row.Cell(9).GetString(),
                                TipoFormaPagoId = 1,
                                ProvinciaId = provinciaId,
                                PoblacionId = poblacionId
                            };

                            parcelas.Add(parcela);
                        }
                        catch (Exception ex)
                        {
                            errores.Add($"Fila {fila}: Error al procesar - {ex.Message}");
                        }

                        fila++;
                    }
                }
            }

            if (errores.Any())
            {
                // Log errors or handle them as needed
                Console.WriteLine("Errores durante la importación:");
                errores.ForEach(e => Console.WriteLine(e));
            }

            return parcelas;
        }
    }
}
