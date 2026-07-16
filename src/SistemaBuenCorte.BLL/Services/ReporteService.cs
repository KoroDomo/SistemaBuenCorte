using Microsoft.EntityFrameworkCore;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.DAL.Data;

namespace SistemaBuenCorte.BLL.Services;

public class ReporteService : IReporteService
{
    private readonly AppDbContext _context;

    public ReporteService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Resumen general: ventas de hoy, ventas del mes, facturas emitidas y producto top.
    /// </summary>
    public async Task<ReporteResumenDto> ObtenerResumenAsync()
    {
        var hoy = DateTime.Today;
        var ayer = hoy.AddDays(-1);
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
        var inicioMesAnterior = inicioMes.AddMonths(-1);
        var finMesAnterior = inicioMes.AddDays(-1);

        // Ventas de hoy
        var ventasHoy = await _context.Ventas
            .Where(v => v.Fecha.Date == hoy)
            .SumAsync(v => (decimal?)v.Total) ?? 0;

        // Ventas de ayer (para calcular variación)
        var ventasAyer = await _context.Ventas
            .Where(v => v.Fecha.Date == ayer)
            .SumAsync(v => (decimal?)v.Total) ?? 0;

        // Ventas del mes actual
        var ventasMes = await _context.Ventas
            .Where(v => v.Fecha >= inicioMes)
            .SumAsync(v => (decimal?)v.Total) ?? 0;

        // Ventas del mes anterior (para calcular variación)
        var ventasMesAnterior = await _context.Ventas
            .Where(v => v.Fecha >= inicioMesAnterior && v.Fecha <= finMesAnterior)
            .SumAsync(v => (decimal?)v.Total) ?? 0;

        // Facturas emitidas en el mes
        var facturasEmitidas = await _context.Facturas
            .Where(f => f.FechaEmision >= inicioMes)
            .CountAsync();

        // Producto más vendido (por cantidad total vendida)
        var productoTop = await _context.DetallesVenta
            .Include(d => d.Producto)
            .GroupBy(d => new { d.ProductoId, d.Producto!.Nombre, d.Producto.TipoVenta })
            .Select(g => new
            {
                g.Key.Nombre,
                g.Key.TipoVenta,
                TotalCantidad = g.Sum(d => d.Cantidad)
            })
            .OrderByDescending(x => x.TotalCantidad)
            .FirstOrDefaultAsync();

        // Calcular variaciones porcentuales
        var variacionDia = ventasAyer > 0
            ? Math.Round((ventasHoy - ventasAyer) / ventasAyer * 100, 1)
            : 0;

        var variacionMes = ventasMesAnterior > 0
            ? Math.Round((ventasMes - ventasMesAnterior) / ventasMesAnterior * 100, 1)
            : 0;

        return new ReporteResumenDto
        {
            VentasHoy = ventasHoy,
            VentasMes = ventasMes,
            FacturasEmitidas = facturasEmitidas,
            ProductoTopNombre = productoTop?.Nombre ?? "Sin datos",
            ProductoTopCantidad = productoTop?.TotalCantidad ?? 0,
            ProductoTopUnidad = productoTop?.TipoVenta == "Peso" ? "kg" : "und",
            VariacionDia = variacionDia,
            VariacionMes = variacionMes
        };
    }

    /// <summary>
    /// Ventas totales de los últimos 7 días para el gráfico de barras.
    /// </summary>
    public async Task<IEnumerable<VentaPorDiaDto>> ObtenerVentasPorDiaAsync()
    {
        var hace7Dias = DateTime.Today.AddDays(-6);

        var ventas = await _context.Ventas
            .Where(v => v.Fecha.Date >= hace7Dias)
            .GroupBy(v => v.Fecha.Date)
            .Select(g => new
            {
                Fecha = g.Key,
                Total = g.Sum(v => v.Total)
            })
            .OrderBy(x => x.Fecha)
            .ToListAsync();

        // Mapeo a DTO con nombre corto del día en español
        var nombres = new[] { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };

        return ventas.Select(v => new VentaPorDiaDto
        {
            Dia = nombres[(int)v.Fecha.DayOfWeek],
            Fecha = v.Fecha,
            Total = v.Total
        });
    }

    /// <summary>
    /// Top 4 productos más vendidos por cantidad total.
    /// </summary>
    public async Task<IEnumerable<ProductoMasVendidoDto>> ObtenerMasVendidosAsync()
    {
        var masVendidos = await _context.DetallesVenta
            .Include(d => d.Producto)
            .GroupBy(d => new { d.ProductoId, d.Producto!.Nombre, d.Producto.TipoVenta })
            .Select(g => new ProductoMasVendidoDto
            {
                Nombre = g.Key.Nombre,
                Cantidad = g.Sum(d => d.Cantidad),
                Unidad = g.Key.TipoVenta == "Peso" ? "kg" : "und"
            })
            .OrderByDescending(x => x.Cantidad)
            .Take(4)
            .ToListAsync();

        return masVendidos;
    }

    /// <summary>
    /// Últimas N ventas con su factura y cajero para el historial reciente.
    /// </summary>
    public async Task<IEnumerable<HistorialVentaDto>> ObtenerHistorialAsync(int cantidad = 10)
    {
        var historial = await _context.Ventas
            .Include(v => v.Factura)
            .Include(v => v.Usuario)
            .Include(v => v.Detalles)
            .OrderByDescending(v => v.Fecha)
            .Take(cantidad)
            .Select(v => new HistorialVentaDto
            {
                Id = v.Id,
                NumeroFactura = v.Factura != null ? v.Factura.NumeroFactura : $"#VEN-{v.Id:D4}",
                FechaHora = v.Fecha.ToString("dd MMM · HH:mm"),
                Cajero = v.Usuario != null ? v.Usuario.NombreCompleto : "Desconocido",
                CantidadProductos = v.Detalles.Count,
                Total = v.Total
            })
            .ToListAsync();

        return historial;
    }
}
