namespace TFG_Cultivos.Models
{
    public class ImportPacRequest
    {
        public int Anio { get; set; }
        public IFormFile ArchivoExcel { get; set; }
    }

}
