using ApiTestePDF.Model;
using ApiTestePDF.Service;
using Microsoft.AspNetCore.Mvc;

public class PdfGenerationController : ControllerBase
{

    private readonly IPDFService _pdfService;
    public PdfGenerationController(IPDFService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpPost]
    [Route("api/pdfgeneration/post")]
    public async Task<IActionResult> GeneratePdfWithAttachments([FromBody] PdfGenerationRequest request)
    {
        try
        {
            string requestId = _pdfService.GeneratePDF(request);
            // Retorna um objeto com o ID de execução para acompanhar o status
            return Ok(new { RequestId = requestId });
        }
        catch (Exception ex)
        {
            // Em caso de erro, retorne um erro interno do servidor
            return Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("api/pdfgeneration/status/{requestId}")]
    public IActionResult GetStatus(string requestId)
    {
        var result = _pdfService.GetPdfStatus(requestId);
        //return o status da solicitação
        return Ok(result);
    }


}
