using ClosedXML.Excel;
using NPOI.HSSF.UserModel;

namespace TFG_Cultivos.Services.ExcelConversionService
{
    public class ExcelConversionService : IExcelConversionService
    {
        public XLWorkbook ConvertToXlsx(IFormFile archivo)
        {
            var extension = Path.GetExtension(archivo.FileName).ToLower();

            using var inputStream = new MemoryStream();
            archivo.CopyTo(inputStream);
            inputStream.Position = 0;

            if (extension == ".xlsx")
            {
                return new XLWorkbook(inputStream);
            }

            if (extension == ".xls")
            {
                var hssf = new HSSFWorkbook(inputStream);
                var sheet = hssf.GetSheetAt(0);

                var workbook = new XLWorkbook();
                var ws = workbook.AddWorksheet("PAC");

                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null) continue;

                    for (int j = 0; j < row.LastCellNum; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null) continue;

                        ws.Cell(i + 1, j + 1).Value = cell.ToString();
                    }
                }

                return workbook;
            }

            throw new NotSupportedException("Formato de Excel no soportado.");
        }
    }

}
