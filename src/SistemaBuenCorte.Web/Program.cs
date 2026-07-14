using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaBuenCorte.BLL.Services;
using SistemaBuenCorte.DAL.Data;
using SistemaBuenCorte.DAL.Entities;

var builder = WebApplication.CreateBuilder(args);

// Base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servicios de negocio
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// JWT - generar secret dinamicamente si es el default
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"] ?? "";

if (string.IsNullOrEmpty(jwtKey) || jwtKey == "CHANGE_THIS_SECRET_KEY")
{
    jwtKey = GenerateSecureKey();
    jwtSettings["Key"] = jwtKey;
    Console.WriteLine("JWT secret generado automaticamente.");
}

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "SistemaBuenCorte",
        ValidAudience = jwtSettings["Audience"] ?? "BuenCorteClientes",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

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

// Seed de datos iniciales
await SeedDataAsync(app.Services);

app.UseCors("PermitirReact");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static string GenerateSecureKey()
{
    using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
    var bytes = new byte[32];
    rng.GetBytes(bytes);
    return Convert.ToBase64String(bytes);
}

static async Task SeedDataAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

    logger.LogInformation("Verificando base de datos...");

    await context.Database.EnsureCreatedAsync();

    if (await context.Usuarios.AnyAsync())
    {
        logger.LogInformation("Base de datos ya tiene usuarios. No se requiere seed.");
        return;
    }

    logger.LogInformation("Creando usuarios iniciales...");

    context.Usuarios.AddRange(
        new Usuario
        {
            NombreCompleto = "Administrador",
            NombreUsuario = "admin",
            ContrasenaHash = "admin123",
            Rol = "Administrador",
            Activo = true,
            FechaCreacion = DateTime.Now
        },
        new Usuario
        {
            NombreCompleto = "Cajero",
            NombreUsuario = "cajero",
            ContrasenaHash = "cajero123",
            Rol = "Cajero",
            Activo = true,
            FechaCreacion = DateTime.Now
        }
    );

    context.Productos.AddRange(
        new Producto
        {
            Nombre = "Carne molida de res",
            Descripcion = "Carne molida fresca 80/20",
            Categoria = "Res",
            TipoVenta = "Peso",
            Precio = 280.00m,
            Stock = 25.500m,
            Activo = true,
            FechaCreacion = DateTime.Now
        },
        new Producto
        {
            Nombre = "Pechuga de pollo",
            Descripcion = "Bandeja de pechuga deshuesada",
            Categoria = "Pollo",
            TipoVenta = "Unidad",
            Precio = 350.00m,
            Stock = 40.000m,
            Activo = true,
            FechaCreacion = DateTime.Now
        }
    );

    context.Descuentos.AddRange(
        new Descuento { Nombre = "Descuento empleado", Tipo = "Porcentaje", Valor = 10.00m, Activo = true },
        new Descuento { Nombre = "Promo del dia", Tipo = "MontoFijo", Valor = 50.00m, Activo = true }
    );

    await context.SaveChangesAsync();

    logger.LogInformation("Seed completado. Credenciales:");
    logger.LogInformation("  Admin: admin / admin123");
    logger.LogInformation("  Cajero: cajero / cajero123");
}
