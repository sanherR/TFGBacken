using TFGBACKEN.Data;
using TFGBACKEN.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext para MySQL
builder.Services.AddDbContext<TfgDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33)) // versión de MySQL
    )
);

// Repositories
builder.Services.AddScoped<UsuarioRepository>();

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();