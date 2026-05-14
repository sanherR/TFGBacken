using Microsoft.EntityFrameworkCore;
using TFGBACKEN.Data;
using TFGBACKEN.Data.Repositories;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TFGBACKEN.Services;
using Microsoft.Extensions.Options;
using TFGBacken.Data.Interfaces;
using TFGBACKEN.data.interfaces;
using TFGBACKEN.data.repository;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IFavoritosRepository, FavoritosRepository>();

builder.Services.AddScoped<IProductosRepository, ProductosRepository>();

builder.Services.AddScoped<IConversacionRepository, ConversacionRepository>();

// ==========================================
// 1. CONFIGURACIÓN DE SERVICIOS (Dependency Injection)
// ==========================================
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,   // <--- Cambiado a false
        ValidateAudience = false, // <--- Cambiado a false
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        // Estas dos líneas de abajo ya no son críticas si pusiste false arriba, 
        // pero déjalas si quieres para no borrar de más
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

        ClockSkew = TimeSpan.Zero 
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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 1. Corta los bucles infinitos (imprescindible)
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        // 2. Ignora nulos para que el móvil no intente leer lo que no existe
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        // 3. Usa los nombres de las variables tal cual (Id, Nombre, etc.)
        options.JsonSerializerOptions.PropertyNamingPolicy = null; 
    });
builder.WebHost.UseUrls("http://0.0.0.0:5062");




// Configurar Swagger/OpenAPI
// Importante: Asegúrate de haber ejecutado: dotnet add package Swashbuckle.AspNetCore
// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "TFG Backend API", 
        Version = "v1",
        Description = "API para gestión de usuarios y productos del TFG" 
    });

    // --- ESTO ES LO QUE TE FALTA PARA EL BOTÓN DEL CANDADO ---
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Escribe: Bearer {tu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
    // --------------------------------------------------------
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