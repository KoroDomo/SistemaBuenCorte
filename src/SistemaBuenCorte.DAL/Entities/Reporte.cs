using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaBuenCorte.DAL.Entities;

/// <summary>
/// Registro de un reporte generado por el administrador. No almacena los
/// resultados calculados (esos se generan al vuelo), sino el rastro de qué
/// reporte se pidió, quién lo generó y para qué rango de fechas. Da soporte
/// al requerimiento de reportes y deja auditoría de su uso.
/// </summary>
[Table("Reportes")]
public class Reporte
{
    [Key]
    public int Id { get; set; }

    /// <summary>Tipo de reporte: "VentasDiarias", "ProductosMasVendidos", etc.</summary>
    [Required]
    [MaxLength(40)]
    public string Tipo { get; set; } = string.Empty;

    /// <summary>Usuario (administrador) que generó el reporte.</summary>
    [Required]
    public int UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }

    /// <summary>Inicio del rango de fechas consultado.</summary>
    public DateTime FechaInicio { get; set; }

    /// <summary>Fin del rango de fechas consultado.</summary>
    public DateTime FechaFin { get; set; }

    /// <summary>Momento en que se generó el reporte.</summary>
    public DateTime FechaGeneracion { get; set; } = DateTime.Now;
}
