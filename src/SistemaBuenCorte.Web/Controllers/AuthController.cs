using Microsoft.AspNetCore.Mvc;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.BLL.Services;

namespace SistemaBuenCorte.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public AuthController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// POST /api/auth/login
    /// Autentica al usuario y devuelve un token JWT con su rol.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var respuesta = await _usuarioService.LoginAsync(dto);
            return Ok(respuesta);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensaje = ex.Message });
        }
    }
}
