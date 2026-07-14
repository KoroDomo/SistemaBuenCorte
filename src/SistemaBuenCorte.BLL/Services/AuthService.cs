using Microsoft.EntityFrameworkCore;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.BLL.Exceptions;
using SistemaBuenCorte.DAL.Data;

namespace SistemaBuenCorte.BLL.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
            errores.Add("El nombre de usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Contrasena))
            errores.Add("La contraseña es obligatoria.");

        if (errores.Count > 0)
            throw new ValidationException(errores);

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u =>
                u.NombreUsuario == dto.NombreUsuario &&
                u.Activo);

        if (usuario is null)
            throw new NotFoundException("Usuario o contraseña incorrectos.");

        // Validación temporal
        if (usuario.ContrasenaHash != dto.Contrasena)
            throw new NotFoundException("Usuario o contraseña incorrectos.");

        return new LoginResponseDto
        {
            NombreCompleto = usuario.NombreCompleto,
            NombreUsuario = usuario.NombreUsuario,
            Rol = usuario.Rol
        };
    }
}