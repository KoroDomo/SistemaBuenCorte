using SistemaBuenCorte.BLL.Services;
using SistemaBuenCorte.DAL.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();

// Base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar el servicio de productos
builder.Services.AddScoped<IProductoService, ProductoService>();

// CORS para React
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Pipeline
app.UseHttpsRedirection();
app.UseCors("PermitirReact");
app.UseAuthorization();
app.MapControllers();

app.Run();