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
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<Caja> Cajas => Set<Caja>();
    public DbSet<Descuento> Descuentos => Set<Descuento>();
    public DbSet<Reporte> Reportes => Set<Reporte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---- Índices únicos ----
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.NombreUsuario)
            .IsUnique();

        modelBuilder.Entity<Factura>()
            .HasIndex(f => f.NumeroFactura)
            .IsUnique();

        // ---- Relación 1 a 1 entre Venta y Factura ----
        modelBuilder.Entity<Factura>()
            .HasOne(f => f.Venta)
            .WithOne(v => v.Factura)
            .HasForeignKey<Factura>(f => f.VentaId);

        // ---- Control de borrado en cascada ----
        // SQL Server no permite múltiples rutas de borrado en cascada hacia la
        // misma tabla (da el error "may cause cycles or multiple cascade paths").
        // Como Venta apunta a Usuario, Caja y Descuento, y otras entidades
        // también apuntan a Usuario, restringimos el borrado en cascada para
        // evitar ese conflicto. En la práctica esto es lo correcto: no quieres
        // que borrar un usuario elimine en cascada todas sus ventas e historial.
        modelBuilder.Entity<Venta>()
            .HasOne(v => v.Usuario)
            .WithMany()
            .HasForeignKey(v => v.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Venta>()
            .HasOne(v => v.Caja)
            .WithMany(c => c.Ventas)
            .HasForeignKey(v => v.CajaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Venta>()
            .HasOne(v => v.Descuento)
            .WithMany(d => d.Ventas)
            .HasForeignKey(v => v.DescuentoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Caja>()
            .HasOne(c => c.Usuario)
            .WithMany()
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DetalleVenta>()
            .HasOne(d => d.Producto)
            .WithMany()
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reporte>()
            .HasOne(r => r.Usuario)
            .WithMany()
            .HasForeignKey(r => r.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cuando se borra una Venta, sí tiene sentido borrar sus líneas de detalle.
        modelBuilder.Entity<DetalleVenta>()
            .HasOne(d => d.Venta)
            .WithMany(v => v.Detalles)
            .HasForeignKey(d => d.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---- Datos semilla ----
        // Usuarios de ejemplo (uno por rol). Las contraseñas son PLACEHOLDERS:
        // el módulo de login (Punto 2) definirá el hash real y deberá
        // regenerarlas para que estos usuarios puedan iniciar sesión.
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                NombreCompleto = "Administrador del Sistema",
                NombreUsuario = "admin",
                ContrasenaHash = "$2a$11$ZiF34myITAxk5LYYp4vIS.aN.zFmicf1eoscUjqFlriTsEKQKWgq6",
                Rol = "Administrador",
                Activo = true,
                FechaCreacion = new DateTime(2026, 1, 1)
            },
            new Usuario
            {
                Id = 2,
                NombreCompleto = "Cajero de Prueba",
                NombreUsuario = "cajero",
                ContrasenaHash = "$2a$11$jEUie1ZnO7uOi.aj0Trki.QkINtTVDqa784n3C/aIiKM2dHPcGXsC",
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

        // Descuentos de ejemplo para que el catálogo no esté vacío.
        modelBuilder.Entity<Descuento>().HasData(
            new Descuento { Id = 1, Nombre = "Descuento empleado", Tipo = "Porcentaje", Valor = 10.00m, Activo = true },
            new Descuento { Id = 2, Nombre = "Promoción del día", Tipo = "MontoFijo", Valor = 50.00m, Activo = true }
        );
    }
}
