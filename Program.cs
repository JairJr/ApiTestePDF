using ApiTestePDF.Data;
using ApiTestePDF.Model;
using ApiTestePDF.Service;
using AutoMapper;
using ElmahCore;
using ElmahCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");

builder.Services.AddDbContext<PDFContext>(opt =>
opt.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? connectionString)); 

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPDFService, PDFService>();
builder.Services.AddSingleton(config => new MapperConfiguration(cfg =>
{
    cfg.CreateMap<PdfDTO, PdfGenerationResponse>();
}).CreateMapper());

builder.Services.AddElmah<XmlFileErrorLog>(options =>
{
    options.LogPath = "~/log";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PDFContext>();
    context.Database.Migrate();
}

//// Ainda estou estudando como funciona com o Elastic APM e questão de licenciamento para dev
//Environment.SetEnvironmentVariable("ELASTIC_APM_SERVER_URLS", "http://localhost:8200");
//Environment.SetEnvironmentVariable("ELASTIC_APM_ENVIRONMENT", "dev");
//Environment.SetEnvironmentVariable("ELASTIC_APM_SERVICE_NAME", "MyApp");

//var config = new ConfigurationBuilder()
//    .AddEnvironmentVariables()                    // Add Environment Variables to configuration 
//    .Build();
//app.UseAllElasticApm(config);
//app.UseElasticApm(config,
//    new HttpDiagnosticsSubscriber(),  /* Enable tracing of outgoing HTTP requests */
//    new EfCoreDiagnosticsSubscriber()); /* Enable tracing of database calls through EF Core*/

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseElmah();
app.Run();
