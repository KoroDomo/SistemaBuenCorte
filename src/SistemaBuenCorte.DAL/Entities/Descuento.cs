using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaBuenCorte.DAL.Entities;

/// <summary>
/// Catálogo de descuentos que el administrador puede gestionar. Una venta
/// puede aplicar opcionalmente uno de estos descuentos. Cumple el
/// requerimiento de descuentos autorizados.
/// </summary>
[Table("Descuentos")]
public class Descuento
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Tipo de descuento: "Porcentaje" o "MontoFijo".</summary>
    [Required]
    [MaxLength(15)]
    public string Tipo { get; set; } = string.Empty;

    /// <summary>
    /// Valor del descuento. Si Tipo es "Porcentaje" es un % (ej. 10.00 = 10%);
    /// si es "MontoFijo" es una cantidad en moneda.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Valor { get; set; }

    public bool Activo { get; set; } = true;

    /// <summary>Ventas que aplicaron este descuento.</summary>
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
