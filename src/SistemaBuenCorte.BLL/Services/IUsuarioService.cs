using SistemaBuenCorte.BLL.DTOs;

namespace SistemaBuenCorte.BLL.Services;

public interface IUsuarioService
{
    /// <summary>
    /// Autentica un usuario y devuelve el token JWT si las credenciales son válidas.
    /// Lanza UnauthorizedAccessException si las credenciales son incorrectas o el usuario está inactivo.
    /// </summary>
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
}
