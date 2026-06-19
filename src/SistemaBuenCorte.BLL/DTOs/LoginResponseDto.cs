namespace SistemaBuenCorte.BLL.DTOs;

/// <summary>
/// Respuesta que el servidor devuelve al autenticarse exitosamente.
/// </summary>
public class LoginResponseDto
{
    public int Id { get; set; } 
    public string Token { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}