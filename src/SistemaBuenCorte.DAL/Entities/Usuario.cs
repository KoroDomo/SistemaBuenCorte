using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaBuenCorte.DAL.Entities;

/// <summary>
/// Representa un usuario del sistema. El rol determina las funciones
/// disponibles (cajero o administrador), cumpliendo el requerimiento
/// de autenticación y control de acceso por roles.
/// </summary>
[Table("Usuarios")]
public class Usuario
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>Nombre de usuario único para iniciar sesión.</summary>
    [Required]
    [MaxLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Hash de la contraseña. NUNCA se almacena la contraseña en texto plano.
    /// El módulo de login (Punto 2) se encargará de hashear y verificar.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string ContrasenaHash { get; set; } = string.Empty;

    /// <summary>Rol del usuario: "Cajero" o "Administrador".</summary>
    [Required]
    [MaxLength(20)]
    public string Rol { get; set; } = string.Empty;

    /// <summary>Permite desactivar un usuario sin eliminarlo.</summary>
    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}
