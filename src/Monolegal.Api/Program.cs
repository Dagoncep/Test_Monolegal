using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Monolegal.Api.Repositories;
using Monolegal.Api.Services;
using Monolegal.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

// MongoDB: los campos se guardan en camelCase (Estado -> "estado") y se ignoran campos extra
ConventionRegistry.Register("Convenciones", new ConventionPack
{
    new CamelCaseElementNameConvention(),
    new IgnoreExtraElementsConvention(true)
}, _ => true);

var mongoSettings = builder.Configuration.GetSection("Mongo").Get<MongoSettings>()!;
var mongoClient = new MongoClient(mongoSettings.ConnectionString);
builder.Services.AddSingleton(mongoClient.GetDatabase(mongoSettings.DatabaseName));

// Configuración del correo, leída de la sección "Smtp" de appsettings.json
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

// Inyección de dependencias: cuando una clase pida la interfaz, recibe esta implementación
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRecordatorioService, RecordatorioService>();
builder.Services.AddScoped<IFacturaService, FacturaService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Cualquier error no controlado se devuelve como un JSON estándar (ProblemDetails)
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opciones => opciones.SwaggerEndpoint("/openapi/v1.json", "Monolegal API"));
}

app.MapControllers();

app.Run();
