using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.BLL.Services;
using SistemaBuenCorte.DAL.Entities;
using SistemaBuenCorte.Tests.Helpers;
using Xunit;

namespace SistemaBuenCorte.Tests;

/// <summary>
/// Pruebas unitarias del módulo de AUTENTICACIÓN (Login).
///
/// Se prueba <see cref="UsuarioService"/>, que es el servicio REAL que usa la API:
/// está registrado en Program.cs y lo consume AuthController. Verifica la
/// contraseña con BCrypt y genera un token JWT.
///
/// (Nota: existe otra clase, AuthService, que NO se usa en la app y que el equipo
/// marcó para eliminar en el registro de incidencias —BUG-07—, por eso NO se prueba.)
///
/// Casos del Plan de Pruebas cubiertos aquí: TC-01, TC-02 y TC-06.
/// Patrón usado en cada prueba: AAA (Arrange - Act - Assert).
/// </summary>
public class AuthTests
{
    /// <summary>
    /// Construye una IConfiguration de prueba con una clave JWT válida.
    /// La clave debe tener al menos 32 bytes (256 bits) porque el token se
    /// firma con HMAC-SHA256; si fuera más corta, la generación del token fallaría.
    /// </summary>
    private static IConfiguration CrearConfiguracion()
    {
        var valores = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "clave-secreta-de-pruebas-para-firmar-tokens-jwt-1234567890",
            ["Jwt:Issuer"] = "SistemaBuenCorteTest",
            ["Jwt:Audience"] = "SistemaBuenCorteTestUsuarios"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(valores)
            .Build();
    }

    /// <summary>
    /// Crea un usuario cajero de prueba. La contraseña se guarda HASHEADA con BCrypt,
    /// igual que en la aplicación real (nunca en texto plano).
    /// </summary>
    private static Usuario CrearUsuarioCajero(string password) => new()
    {
        Id = 1,
        NombreCompleto = "Johan Cáceres",
        NombreUsuario = "jcaceres",
        ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(password),
        Rol = "Cajero",
        Activo = true,
        FechaCreacion = new DateTime(2026, 1, 1)
    };

    // ---------------------------------------------------------------------
    // TC-01 | Login con usuario válido -> acceso concedido (devuelve datos + token)
    // ---------------------------------------------------------------------
    [Fact]
    public async Task TC01_Login_ConUsuarioValido_RetornaUsuarioYToken()
    {
        // Arrange
        using var context = TestDbContextFactory.Crear();
        context.Usuarios.Add(CrearUsuarioCajero("1234"));
        await context.SaveChangesAsync();

        var service = new UsuarioService(context, CrearConfiguracion());
        var dto = new LoginDto { NombreUsuario = "jcaceres", Contrasena = "1234" };

        // Act
        var resultado = await service.LoginAsync(dto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("jcaceres", resultado.NombreUsuario);
        Assert.Equal("Cajero", resultado.Rol);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Token));
    }

    // ---------------------------------------------------------------------
    // TC-02 | Contraseña incorrecta -> se rechaza con el mensaje de error esperado
    // ---------------------------------------------------------------------
    [Fact]
    public async Task TC02_Login_ConContrasenaIncorrecta_LanzaUnauthorized()
    {
        // Arrange
        using var context = TestDbContextFactory.Crear();
        context.Usuarios.Add(CrearUsuarioCajero("1234"));
        await context.SaveChangesAsync();

        var service = new UsuarioService(context, CrearConfiguracion());
        var dto = new LoginDto { NombreUsuario = "jcaceres", Contrasena = "wrong" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(dto));

        Assert.Equal("Usuario o contraseña incorrectos.", ex.Message);
    }

    // ---------------------------------------------------------------------
    // TC-06 | Login con campos vacíos -> la validación se activa
    //         Se prueban las reglas [Required] del LoginDto. Estas son las mismas
    //         que ASP.NET Core (por el atributo [ApiController]) aplica de forma
    //         automática para responder 400 cuando faltan usuario o contraseña.
    // ---------------------------------------------------------------------
    [Fact]
    public void TC06_Login_ConCamposVacios_LaValidacionDetectaErrores()
    {
        // Arrange
        var dto = new LoginDto { NombreUsuario = "", Contrasena = "" };
        var contexto = new ValidationContext(dto);
        var resultados = new List<ValidationResult>();

        // Act
        bool esValido = Validator.TryValidateObject(
            dto, contexto, resultados, validateAllProperties: true);

        // Assert
        Assert.False(esValido);              // no pasa la validación
        Assert.Equal(2, resultados.Count);   // usuario y contraseña son obligatorios
    }
}
