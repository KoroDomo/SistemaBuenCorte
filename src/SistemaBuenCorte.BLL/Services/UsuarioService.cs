using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.DAL.Data;

namespace SistemaBuenCorte.BLL.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public UsuarioService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == dto.NombreUsuario && u.Activo);

        if (usuario is null)
            throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");

        // Verificar contraseña con BCrypt
        bool passwordValida = BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.ContrasenaHash);
        if (!passwordValida)
            throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");

        var token = GenerarToken(usuario.NombreUsuario, usuario.Rol, usuario.Id);

        return new LoginResponseDto
        {
            Token = token,
            NombreUsuario = usuario.NombreUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Rol = usuario.Rol
        };
    }

    private string GenerarToken(string nombreUsuario, string rol, int id)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, nombreUsuario),
            new Claim(ClaimTypes.Name, nombreUsuario),
            new Claim(ClaimTypes.Role, rol),
            new Claim("id", id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
