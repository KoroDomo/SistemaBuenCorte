using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaBuenCorte.DAL.Entities;

/// <summary>
/// Representa un producto del inventario de la carnicería.
/// Soporta el detalle clave del negocio: venta por peso (ej. carne por kg)
/// o por unidad (ej. una pieza), definido en <see cref="TipoVenta"/>.
/// </summary>
[Table("Productos")]
public class Producto
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Descripcion { get; set; }

    [MaxLength(50)]
    public string? Categoria { get; set; }

    /// <summary>Tipo de venta: "Peso" o "Unidad".</summary>
    [Required]
    [MaxLength(10)]
    public string TipoVenta { get; set; } = string.Empty;

    /// <summary>
    /// Precio del producto. Si TipoVenta es "Peso", es el precio por kg;
    /// si es "Unidad", es el precio por pieza.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }

    /// <summary>
    /// Existencia actual. Se usa decimal para soportar productos por peso
    /// (ej. 12.5 kg) además de productos por unidad (ej. 30).
    /// </summary>
    [Column(TypeName = "decimal(10,3)")]
    public decimal Stock { get; set; }

    /// <summary>Permite ocultar un producto sin borrarlo del historial.</summary>
    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
