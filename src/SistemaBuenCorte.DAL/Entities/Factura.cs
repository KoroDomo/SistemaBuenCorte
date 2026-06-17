using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaBuenCorte.DAL.Entities;

/// <summary>
/// Documento de factura emitido a partir de una venta (relación 1 a 1).
/// Separar Factura de Venta refleja la realidad del negocio: la venta es la
/// transacción y la factura es el comprobante legal que se emite.
/// </summary>
[Table("Facturas")]
public class Factura
{
    [Key]
    public int Id { get; set; }

    /// <summary>Número de factura visible al cliente (ej. "FAC-000123").</summary>
    [Required]
    [MaxLength(20)]
    public string NumeroFactura { get; set; } = string.Empty;

    /// <summary>Venta de la que proviene esta factura (relación 1 a 1).</summary>
    [Required]
    public int VentaId { get; set; }

    [ForeignKey(nameof(VentaId))]
    public Venta? Venta { get; set; }

    public DateTime FechaEmision { get; set; } = DateTime.Now;

    /// <summary>Monto gravado/base antes de impuestos.</summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal Subtotal { get; set; }

    /// <summary>Impuesto aplicado (ej. ITBIS). 0 si no aplica.</summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal Impuesto { get; set; }

    /// <summary>Total final de la factura.</summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }

    /// <summary>Nombre del cliente (opcional; muchas ventas son al contado anónimas).</summary>
    [MaxLength(120)]
    public string? NombreCliente { get; set; }
}
