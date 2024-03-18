using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ApiTestePDF.Model
{
    public class PdfDTO
    {        
        public int id { get; set; }
        public string? RequestId { get; set; }
        public string? Status { get; set; }
        public byte[]? PdfContent { get; set; }
    }

    public class PdfGenerationRequest
    {
        public string HtmlContent { get; set; }
        public string[]? PdfPathsToAdd { get; set; }
        public string? CallbackUrl { get; set; }
    }

    public class PdfGenerationResponse
    {
        public string RequestId { get; set; }
        public string? Status { get; set; }
        public byte[]? PdfContent { get; set; }
    }

}
