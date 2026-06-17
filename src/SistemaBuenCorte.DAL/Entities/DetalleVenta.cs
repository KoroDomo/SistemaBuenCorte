using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaBuenCorte.DAL.Entities;

/// <summary>
/// Representa una línea dentro de una venta: un producto con su cantidad y
/// precio. Soporta venta por peso (ej. 1.250 kg) o por unidad (ej. 3) usando
/// un decimal para la cantidad. Se guarda el precio al momento de la venta
/// para que el historial no cambie si luego se actualiza el precio del producto.
/// </summary>
[Table("DetallesVenta")]
public class DetalleVenta
{
    [Key]
    public int Id { get; set; }

    /// <summary>Venta a la que pertenece esta línea.</summary>
    [Required]
    public int VentaId { get; set; }

    [ForeignKey(nameof(VentaId))]
    public Venta? Venta { get; set; }

    /// <summary>Producto vendido en esta línea.</summary>
    [Required]
    public int ProductoId { get; set; }

    [ForeignKey(nameof(ProductoId))]
    public Producto? Producto { get; set; }

    /// <summary>
    /// Cantidad vendida. Decimal para soportar peso (1.250 kg) o unidades (3).
    /// </summary>
    [Column(TypeName = "decimal(10,3)")]
    public decimal Cantidad { get; set; }

    /// <summary>
    /// Precio unitario CONGELADO al momento de la venta. No se toma del
    /// producto en tiempo real para preservar la integridad del historial.
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioUnitario { get; set; }

    /// <summary>Subtotal de la línea (Cantidad * PrecioUnitario).</summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal Subtotal { get; set; }
}
