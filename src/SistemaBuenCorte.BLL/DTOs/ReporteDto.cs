namespace SistemaBuenCorte.BLL.DTOs;

/// <summary>
/// Resumen general del negocio para las 4 tarjetas del dashboard de reportes.
/// </summary>
public class ReporteResumenDto
{
    public decimal VentasHoy { get; set; }
    public decimal VentasMes { get; set; }
    public int FacturasEmitidas { get; set; }
    public string ProductoTopNombre { get; set; } = string.Empty;
    public decimal ProductoTopCantidad { get; set; }
    public string ProductoTopUnidad { get; set; } = string.Empty;
    public decimal VariacionDia { get; set; }   // % vs ayer
    public decimal VariacionMes { get; set; }   // % vs mes anterior
}

/// <summary>
/// Total de ventas de un día específico para el gráfico de barras.
/// </summary>
public class VentaPorDiaDto
{
    public string Dia { get; set; } = string.Empty;   // "Lun", "Mar", etc.
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
}

/// <summary>
/// Producto más vendido por cantidad para la sección "Más vendidos".
/// </summary>
public class ProductoMasVendidoDto
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string Unidad { get; set; } = string.Empty;  // "kg" o "und"
}

/// <summary>
/// Fila del historial de ventas recientes.
/// </summary>
public class HistorialVentaDto
{
    public int Id { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public string FechaHora { get; set; } = string.Empty;
    public string Cajero { get; set; } = string.Empty;
    public int CantidadProductos { get; set; }
    public decimal Total { get; set; }
}
