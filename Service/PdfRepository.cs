using Dapper;
using Microsoft.Data.SqlClient;


namespace ApiTestePDF.Service
{
    public static class PdfRepository
    {
        private static readonly string _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=fase5;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        //public PdfRepository(string connectionString)
        //{
        //    _connectionString = connectionString;
        //}

        public static void UpdatePdfStatus(string requestId, byte[] payload)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"UPDATE PdfTable SET Status = @Status, PdfContent = @PdfContent WHERE RequestId = @RequestId";
                var parameters = new { Status = "completed", PdfContent = payload, RequestId = requestId };
                connection.Execute(sql, parameters);             
            }
        }
    }
}
