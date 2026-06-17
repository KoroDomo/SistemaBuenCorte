using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaBuenCorte.DAL.Entities;

/// <summary>
/// Representa una sesión de caja (un turno). El cajero la abre con un monto
/// inicial y la cierra al final del turno con el arqueo. Cumple el
/// requerimiento de apertura y cierre de caja.
/// </summary>
[Table("Cajas")]
public class Caja
{
    [Key]
    public int Id { get; set; }

    /// <summary>Usuario (cajero) que abrió la caja.</summary>
    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }

    /// <summary>Monto en efectivo con el que se abre la caja.</summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal MontoApertura { get; set; }

    /// <summary>Efectivo contado físicamente al cerrar. Null si sigue abierta.</summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal? MontoCierreContado { get; set; }

    public DateTime FechaApertura { get; set; } = DateTime.Now;

    /// <summary>Fecha de cierre. Null mientras la caja está abierta.</summary>
    public DateTime? FechaCierre { get; set; }

    /// <summary>Estado de la caja: "Abierta" o "Cerrada".</summary>
    [Required]
    [MaxLength(10)]
    public string Estado { get; set; } = "Abierta";

    /// <summary>Ventas registradas durante este turno de caja.</summary>
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
