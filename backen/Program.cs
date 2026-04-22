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
// 1. CONFIGURACIÓN DE SEGURIDAD (JWT)
// ==========================================
// Como appsettings.json está vacío, definimos una clave manual para que no falle
var manualKey = "EstaEsUnaClaveSuperSecretaDePrueba1234567890";
var keyBytes = Encoding.UTF8.GetBytes(manualKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, 
        ValidateAudience = false,
        ValidateLifetime = false, // Ni siquiera mirará si ha caducado
        ValidateIssuerSigningKey = false, // NO comprobará si la clave es la misma
        // Esto hará que CUALQUIER token que parezca un JWT sea aceptado
        SignatureValidator = delegate (string token, TokenValidationParameters parameters)
        {
            return new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token);
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Forzamos la URL de escucha
builder.WebHost.UseUrls("http://0.0.0.0:5062");

// ==========================================
// 2. BASE DE DATOS (CADENA DIRECTA)
// ==========================================
// ⚠️ CAMBIA 'tfg_db' por el nombre real de tu base de datos en MySQL
string connectionString = "server=localhost;database=tfg;user=root;password=;";

builder.Services.AddDbContext<TfgDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// ==========================================
// 3. CONFIGURACIÓN DE SWAGGER Y LÍMITES
// ==========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TFG Backend API", Version = "v1" });

    // Configuración para el botón Authorize de Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT así: Bearer {tu_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50_000_000; // 50 MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 50_000_000;
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
});

// ==========================================
// 4. INYECCIÓN DE DEPENDENCIAS
// ==========================================
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<JwtService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// ==========================================
// 5. PIPELINE (MIDDLEWARE)
// ==========================================

// Forzamos Swagger siempre para que puedas probar
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TFG API v1");
    c.RoutePrefix = "swagger"; 
});

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();