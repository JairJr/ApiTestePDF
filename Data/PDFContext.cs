using ApiTestePDF.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiTestePDF.Data
{
    public class PDFContext : DbContext
    {
        public PDFContext(DbContextOptions<PDFContext> options) : base(options) { }

        public DbSet<PdfDTO> PdfTable { get; set; }
    }
}
