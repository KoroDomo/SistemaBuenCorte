using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SistemaBuenCorte.DAL.Data;

/// <summary>
/// Permite que las herramientas de EF Core (dotnet ef) construyan el
/// AppDbContext en TIEMPO DE DISEÑO, es decir, al ejecutar los comandos de
/// migración. Como la capa DAL es una biblioteca de clases y no tiene
/// appsettings.json, aquí se define la cadena de conexión usada SOLO por
/// las herramientas de migración.
///
/// Cuando el equipo agregue la capa Web (MVC/Razor/Blazor), esa capa
/// configurará el DbContext con su propia cadena de conexión desde
/// appsettings. Esta fábrica NO afecta la app en ejecución.
///
/// Cadena de conexión: usa LocalDB. Como LocalDB es idéntico en todas las
/// máquinas (no depende del nombre del PC), todos los integrantes pueden
/// usar exactamente esta misma cadena sin cambiar nada.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            "Server=(localdb)\\MSSQLLocalDB;" +
            "Database=ElBuenCorte;" +
            "Trusted_Connection=True;" +
            "MultipleActiveResultSets=true;" +
            "TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
