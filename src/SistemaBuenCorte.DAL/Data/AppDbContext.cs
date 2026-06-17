using Microsoft.EntityFrameworkCore;
using SistemaBuenCorte.DAL.Entities;

namespace SistemaBuenCorte.DAL.Data;

/// <summary>
/// Contexto de base de datos de EF Core. Es el puente entre las entidades
/// de C# y las tablas de SQL Server. Cada DbSet expone una tabla.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Índices únicos para evitar duplicados a nivel de base de datos.
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.NombreUsuario)
            .IsUnique();

        // Datos semilla: dos usuarios de ejemplo (uno por rol) y dos productos.
        // Esto da al equipo datos con los que trabajar desde el primer momento.
        // NOTA: las contraseñas aquí son PLACEHOLDERS. El módulo de login
        // (Punto 2) definirá el algoritmo de hash real y deberá regenerarlas.
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                NombreCompleto = "Administrador del Sistema",
                NombreUsuario = "admin",
                ContrasenaHash = "PLACEHOLDER_CAMBIAR",
                Rol = "Administrador",
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1)
            },
            new Usuario
            {
                Id = 2,
                NombreCompleto = "Cajero de Prueba",
                NombreUsuario = "cajero",
                ContrasenaHash = "PLACEHOLDER_CAMBIAR",
                Rol = "Cajero",
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1)
            }
        );

        modelBuilder.Entity<Producto>().HasData(
            new Producto
            {
                Id = 1,
                Nombre = "Carne molida de res",
                Descripcion = "Carne molida fresca, 80/20",
                Categoria = "Res",
                TipoVenta = "Peso",
                Precio = 280.00m,
                Stock = 25.500m,
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1)
            },
            new Producto
            {
                Id = 2,
                Nombre = "Pechuga de pollo (bandeja)",
                Descripcion = "Bandeja de pechuga deshuesada",
                Categoria = "Pollo",
                TipoVenta = "Unidad",
                Precio = 350.00m,
                Stock = 40,
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1)
            }
        );
    }
}
