using ApiTestePDF.Model;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Newtonsoft.Json;
using System.Text;
using iTextSharp.text;
using ApiTestePDF.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiTestePDF.Service
{
    public class PDFService : IPDFService
    {
        public PDFContext _context;
        private readonly IMapper _mapper;

        public PDFService(PDFContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public string GeneratePDF(PdfGenerationRequest item)
        {
            string requestId = Guid.NewGuid().ToString();

            // Salve o ID da solicitação e o status atual no banco de dados
            _context.PdfTable.Add(new PdfDTO { RequestId = requestId, Status = "processing" });            
            _context.SaveChanges();
            // Inicie a geração do PDF em uma nova tarefa assíncrona
            _ = Task.Run(async () =>
            {
                // Simule um processo de geração de PDF
                await Task.Delay(5000); // Simula uma operação demorada

                // Gere o PDF a partir de um HTML
                var pdfFile = GeneratePdfFromHtml(item.HtmlContent); // ("<h1>Exemplo de PDF gerado a partir de HTML</h1>");

                // Adicione outros PDFs
                if (item.PdfPathsToAdd.Length > 0)
                {
                    pdfFile = AddPdfsToPdf(pdfFile, item.PdfPathsToAdd);
                }
                // Após a conclusão, notifique o cliente sobre o status atual
                NotifyClient(requestId, "completed", item.CallbackUrl, pdfFile);

            });
            return requestId;
        }

        public PdfGenerationResponse GetPdfStatus(string requestId)
        {
            var response = _context.PdfTable.Where(x => x.RequestId == requestId).FirstOrDefault();
            return _mapper.Map<PdfGenerationResponse>(response);
        }
        private byte[] GeneratePdfFromHtml(string htmlContent)
        {
            //Magia para converter HTML em PDF
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document();
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();
                using (var sr = new StringReader(htmlContent))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr);
                }
                document.Close();

                // Retorna o conteúdo do MemoryStream como um array de bytes
                return ms.ToArray();
            }
        }

        private byte[] AddPdfsToPdf(byte[] basePdf, string[] pdfPathsToAdd)
        {
            var reader = new PdfReader(basePdf);
            using (var outputStream = new MemoryStream())
            {
                var stamper = new PdfStamper(reader, outputStream);
                int totalPages = reader.NumberOfPages;
                //Itera sobre os PDFs a serem adicionados
                foreach (var pdfPath in pdfPathsToAdd)
                {
                    var pdfReader = new PdfReader(pdfPath);
                    //para cada página do PDF a ser adicionado, insira uma nova página no PDF base a partir da ultima página (totalPages)
                    for (var i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        totalPages++;
                        stamper.InsertPage(totalPages, pdfReader.GetPageSizeWithRotation(i));
                        var pdfContentByte = stamper.GetUnderContent(totalPages);
                        var importedPage = stamper.GetImportedPage(pdfReader, i);
                        pdfContentByte.AddTemplate(importedPage, 0, 0);
                    }
                }
                stamper.Close();
                reader.Close();
                return outputStream.ToArray();
            }
        }

        private void NotifyClient(string requestId, string status, string callbackUrl, byte[] payload)
        {
            PdfRepository.UpdatePdfStatus(requestId,payload);
            //implementar a lógica para notificar o cliente sobre o status atual com base no callbackUrl
            if (String.IsNullOrEmpty(callbackUrl))
            {
                ///Apenas logamos as informações e salvamos o arquivo localmente 
                Console.WriteLine($"Status update for request ID '{requestId}': {status}. Callback URL: {callbackUrl}");
                Console.WriteLine($"Payload: {Encoding.UTF8.GetString(payload)}");
                string path = Directory.GetCurrentDirectory();
                System.IO.File.WriteAllBytes($"{path}\\{requestId}.pdf", payload);
            }
            else
            {
                //Enviamos a notificação para o callbackUrl
                SendNotification(requestId, status, callbackUrl, payload);
            }
        }

        private void SendNotification(string requestId, string status, string callbackUrl, byte[] payload)
        {
            var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(new PdfGenerationResponse { RequestId = requestId, Status = status, PdfContent = payload }), Encoding.UTF8, "application/json");
            var response = client.PostAsync(callbackUrl, content).Result;
        }
    }
}
