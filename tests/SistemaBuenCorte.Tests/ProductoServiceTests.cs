using Microsoft.EntityFrameworkCore;
using SistemaBuenCorte.BLL.DTOs;
using SistemaBuenCorte.BLL.Exceptions;
using SistemaBuenCorte.BLL.Services;
using SistemaBuenCorte.DAL.Entities;
using SistemaBuenCorte.Tests.Helpers;
using Xunit;

namespace SistemaBuenCorte.Tests;

/// <summary>
/// Pruebas unitarias del módulo de PRODUCTOS.
///
/// Se prueba <see cref="ProductoService"/>, el servicio real de la capa de negocio
/// (BLL) que contiene las reglas de validación (precio > 0, nombre obligatorio,
/// nombre no duplicado, tipo de venta válido) y el borrado lógico.
///
/// Casos del Plan de Pruebas cubiertos aquí: TC-04, TC-05, TC-07 y TC-08.
/// Patrón usado en cada prueba: AAA (Arrange - Act - Assert).
/// </summary>
public class ProductoServiceTests
{
    /// <summary>Crea un producto de prueba ya válido para sembrar en la BD.</summary>
    private static Producto CrearProductoDemo(int id, string nombre) => new()
    {
        Id = id,
        Nombre = nombre,
        Descripcion = "Producto de prueba",
        Categoria = "Res",
        TipoVenta = "Peso",
        Precio = 300.00m,
        Stock = 10m,
        Activo = true,
        FechaCreacion = new DateTime(2026, 1, 1)
    };

    // ---------------------------------------------------------------------
    // TC-04 | Agregar producto con datos válidos -> se guarda y devuelve un Id > 0
    // ---------------------------------------------------------------------
    [Fact]
    public async Task TC04_Crear_ConDatosValidos_GuardaProductoYRetornaId()
    {
        // Arrange
        using var context = TestDbContextFactory.Crear();
        var service = new ProductoService(context);
        var dto = new ProductoCreateDto
        {
            Nombre = "Bistec de res",
            Descripcion = "Corte fresco",
            Categoria = "Res",
            TipoVenta = "Peso",
            Precio = 385.00m,
            Stock = 20m
        };

        // Act
        var creado = await service.CrearAsync(dto);

        // Assert
        Assert.NotNull(creado);
        Assert.True(creado.Id > 0);
        Assert.Equal("Bistec de res", creado.Nombre);
        Assert.Equal(1, await context.Productos.CountAsync());
    }

    // ---------------------------------------------------------------------
    // TC-05 | Editar el precio de un producto existente -> precio actualizado en BD
    // ---------------------------------------------------------------------
    [Fact]
    public async Task TC05_Actualizar_ConNuevoPrecio_ActualizaElProducto()
    {
        // Arrange
        using var context = TestDbContextFactory.Crear();
        context.Productos.Add(CrearProductoDemo(1, "Costilla de cerdo"));
        await context.SaveChangesAsync();

        var service = new ProductoService(context);
        var dto = new ProductoUpdateDto
        {
            Nombre = "Costilla de cerdo",
            Descripcion = "Costilla para asar",
            Categoria = "Cerdo",
            TipoVenta = "Peso",
            Precio = 400.00m,   // precio nuevo
            Stock = 10m,
            Activo = true
        };

        // Act
        var actualizado = await service.ActualizarAsync(1, dto);

        // Assert
        Assert.Equal(400.00m, actualizado.Precio);

        var enBd = await context.Productos.FindAsync(1);
        Assert.Equal(400.00m, enBd!.Precio);
    }

    // ---------------------------------------------------------------------
    // TC-07 | Eliminar un producto existente -> BORRADO LÓGICO (Activo = false)
    //         El producto NO se borra físicamente para no perder el historial de ventas.
    // ---------------------------------------------------------------------
    [Fact]
    public async Task TC07_Eliminar_ProductoExistente_LoMarcaComoInactivo()
    {
        // Arrange
        using var context = TestDbContextFactory.Crear();
        context.Productos.Add(CrearProductoDemo(1, "Chorizo artesanal"));
        await context.SaveChangesAsync();

        var service = new ProductoService(context);

        // Act
        await service.EliminarAsync(1);

        // Assert
        var enBd = await context.Productos.FindAsync(1);
        Assert.NotNull(enBd);          // sigue existiendo (no se borra físicamente)
        Assert.False(enBd!.Activo);    // pero queda marcado como inactivo
    }

    // ---------------------------------------------------------------------
    // TC-08 | Agregar producto con precio inválido (negativo) -> ValidationException
    // ---------------------------------------------------------------------
    [Fact]
    public async Task TC08_Crear_ConPrecioInvalido_LanzaValidationException()
    {
        // Arrange
        using var context = TestDbContextFactory.Crear();
        var service = new ProductoService(context);
        var dto = new ProductoCreateDto
        {
            Nombre = "Costillas de cerdo",
            TipoVenta = "Peso",
            Precio = -50.00m,   // precio inválido
            Stock = 5m
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => service.CrearAsync(dto));

        Assert.Contains("El precio debe ser mayor a cero.", ex.Errores);
    }
}
