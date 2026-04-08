using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Repositories;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURACIÓN DE SERVICIOS (Dependency Injection)
// ==========================================

builder.Services.AddControllers();

// Configurar Swagger/OpenAPI
// Importante: Asegúrate de haber ejecutado: dotnet add package Swashbuckle.AspNetCore
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "TFG Backend API", 
        Version = "v1",
        Description = "API para gestión de usuarios y productos del TFG" 
    });
});

// Configurar DbContext para MySQL
builder.Services.AddDbContext<TfgDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50_000_000; // 50 MB
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 50_000_000; // 50 MB
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5); // Evita cierre prematuro de socket
});

// Inyección de Repositorios
builder.Services.AddScoped<UsuarioRepository>();
// Si tienes un ProductoRepository, añádelo aquí también:
// builder.Services.AddScoped<ProductoRepository>();

// Configuración de CORS (Permite que el Frontend se conecte)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// ==========================================
// 2. CONFIGURACIÓN DEL PIPELINE (Middleware)
// ==========================================

// Habilitar Swagger siempre en desarrollo para probar fácilmente
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TFG API v1");
        c.RoutePrefix = "swagger"; // Esto hace que entres por http://localhost:5062/swagger
    });
}

// Redirección HTTPS opcional (puedes comentarlo si da problemas en local)
// app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Urls.Add("http://0.0.0.0:5062");


app.Run();