using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaBuenCorte.DAL.Entities;

/// <summary>
/// Representa una venta (transacción). Contiene las líneas de productos
/// vendidos (DetalleVenta), el cajero que la registró, la caja del turno y,
/// opcionalmente, un descuento aplicado. Cumple el requerimiento de registro
/// de ventas con cálculo automático del total.
/// </summary>
[Table("Ventas")]
public class Venta
{
    [Key]
    public int Id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    /// <summary>Cajero que registró la venta.</summary>
    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }

    /// <summary>Caja (turno) en la que se registró la venta.</summary>
    [Required]
    public int CajaId { get; set; }

    [ForeignKey(nameof(CajaId))]
    public Caja? Caja { get; set; }

    /// <summary>Descuento aplicado a la venta (opcional).</summary>
    public int? DescuentoId { get; set; }

    [ForeignKey(nameof(DescuentoId))]
    public Descuento? Descuento { get; set; }

    /// <summary>Suma de los subtotales de las líneas, antes del descuento.</summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal Subtotal { get; set; }

    /// <summary>Monto descontado (calculado a partir del descuento aplicado).</summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal MontoDescuento { get; set; }

    /// <summary>Total final a pagar (Subtotal - MontoDescuento, + impuestos si aplica).</summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }

    /// <summary>Método de pago: "Efectivo" o "Tarjeta".</summary>
    [Required]
    [MaxLength(15)]
    public string MetodoPago { get; set; } = "Efectivo";

    /// <summary>Líneas de detalle de la venta (productos vendidos).</summary>
    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

    /// <summary>Factura generada a partir de esta venta (relación 1 a 1).</summary>
    public Factura? Factura { get; set; }
}
