# SistemaBuenCorte.Tests

Proyecto de **pruebas unitarias** (xUnit) del sistema "El Buen Corte" — Entregable 4.

Prueba la capa de negocio (BLL) usando una base de datos **en memoria** (EF Core InMemory),
por lo que **no necesita SQL Server** para ejecutarse.

## Contenido

- `Helpers/TestDbContextFactory.cs` — crea un `AppDbContext` en memoria y aislado por prueba.
- `AuthTests.cs` — módulo de autenticación (login). Casos **TC-01, TC-02, TC-06**.
- `ProductoServiceTests.cs` — módulo de productos. Casos **TC-04, TC-05, TC-07, TC-08**.

Total: **7 pruebas unitarias** (el mínimo del entregable es 5).

## Cómo ejecutar

En Visual Studio: menú **Prueba → Explorador de pruebas → Ejecutar todo**.

Por consola, desde la carpeta de este proyecto:

```bash
dotnet test
```

Todas las pruebas deben salir en **verde (PASS)**.
