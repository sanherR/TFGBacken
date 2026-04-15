using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Repositories;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TFGBACKEN.Services;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURACIÓN DE SERVICIOS (Dependency Injection)
// ==========================================
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

        ClockSkew = TimeSpan.Zero // 🔥 IMPORTANTE (evita errores raros de expiración)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("JWT ERROR: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine(" JWT CHALLENGE: " + context.ErrorDescription);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.WebHost.UseUrls("http://0.0.0.0:5062");




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
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50_000_000; // 50 MB
});

// Configurar DbContext para MySQL
builder.Services.AddDbContext<TfgDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 50_000_000; // 50 MB
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5); // Evita cierre prematuro de socket
});

// Inyección de Repositorios

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<JwtService>();
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
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();


app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();


app.Run();