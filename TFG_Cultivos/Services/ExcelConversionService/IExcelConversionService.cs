using ClosedXML.Excel;

namespace TFG_Cultivos.Services.ExcelConversionService
{
    public interface IExcelConversionService
    {
        XLWorkbook ConvertToXlsx(IFormFile archivo);
    }
}

