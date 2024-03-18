using ApiTestePDF.Model;

namespace ApiTestePDF.Service
{
    public interface IPDFService
    {
        string GeneratePDF(PdfGenerationRequest item);
        PdfGenerationResponse GetPdfStatus(string requestId);
    }
}
