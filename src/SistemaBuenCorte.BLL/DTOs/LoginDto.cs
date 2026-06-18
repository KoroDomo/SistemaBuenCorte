using System.ComponentModel.DataAnnotations;

namespace SistemaBuenCorte.BLL.DTOs;

/// <summary>
/// Datos que el cliente envía para iniciar sesión.
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Contrasena { get; set; } = string.Empty;
}