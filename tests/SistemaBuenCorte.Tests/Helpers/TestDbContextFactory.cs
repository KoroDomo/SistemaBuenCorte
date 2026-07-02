using Microsoft.EntityFrameworkCore;
using SistemaBuenCorte.DAL.Data;

namespace SistemaBuenCorte.Tests.Helpers;

/// <summary>
/// Crea un <see cref="AppDbContext"/> usando la base de datos EN MEMORIA de EF Core.
///
/// ¿Por qué en memoria? Las pruebas unitarias deben ser rápidas y no depender de
/// una base de datos real (SQL Server LocalDB). Con el proveedor InMemory, cada
/// prueba trabaja sobre una BD temporal que vive solo durante la prueba.
///
/// Cada llamada usa un nombre único (Guid) para que las pruebas queden totalmente
/// AISLADAS entre sí: lo que una prueba guarda no contamina a las demás.
/// </summary>
public static class TestDbContextFactory
{
    public static AppDbContext Crear()
    {
        var opciones = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"BuenCorte_Test_{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(opciones);
    }
}
