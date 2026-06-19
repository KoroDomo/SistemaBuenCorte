# SistemaBuenCorte
Sistema de facturación e inventario para la carnicería El Buen Corte. Proyecto de Programación III.

## Resumen
Aplicación web multicapa (API .NET + React) para gestionar productos, inventario y ventas. Usa SQL Server LocalDB y Entity Framework Core. Este README contiene pasos reproducibles para ejecutar la solución localmente.

## Tecnologías
- C# / .NET 9 (net9.0)
- ASP.NET Core
- Entity Framework Core 9
- SQL Server LocalDB
- React (Create React App)
- Node.js

## Requisitos y verificación
- .NET 9 SDK (comprobar con `dotnet --version`; se requiere 9.x)
- SQL Server LocalDB (instancia `(localdb)\\MSSQLLocalDB`). LocalDB es un componente del sistema, no está incluido en el repositorio; instalar SQL Server Express LocalDB o Visual Studio.
- Node.js 18+ y npm
- dotnet-ef (opcional, para migraciones): `dotnet tool install --global dotnet-ef`
- Git

Verificación rápida (ejecuta antes de continuar):
```powershell
dotnet --version
sqllocaldb info MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
node --version
npm --version
git --version
```

Recomendaciones rápidas:
- LocalDB debe estar instalado y ejecutándose en la máquina (el repositorio no contiene el motor de base de datos). Ejecutar `sqllocaldb start MSSQLLocalDB` antes de aplicar migraciones.
- Usar `npm ci` para instalaciones reproducibles en entornos de evaluación; `npm install` está bien para desarrollo local.
- Instalar `dotnet-ef` para aplicar migraciones localmente si se van a ejecutar: `dotnet tool install --global dotnet-ef`.
- Si Git en Windows da errores por longitud de rutas, habilitar rutas largas: `git config --system core.longpaths true`.
- Detener servidores (`dotnet run`, `npm start`) antes de ejecutar `dotnet ef database update`.

Si prefieres usar Docker en vez de LocalDB, ajustar la cadena de conexión en `appsettings.Development.json` y documentar el contenedor a usar.

## Estructura
```
SistemaBuenCorte/
├── SistemaBuenCorte.sln
├── src/
│   ├── SistemaBuenCorte.Web/    # API + proyecto web
│   ├── SistemaBuenCorte.BLL/    # Lógica de negocio
│   └── SistemaBuenCorte.DAL/    # DbContext, entidades, migraciones
```

## Pasos para ejecutar (desde la raíz del repositorio)
1) Asegurar `develop` actualizado:
```bash
git checkout develop
git pull origin develop
```

2) Iniciar LocalDB (si no está):
```powershell
sqllocaldb start MSSQLLocalDB
```

3) Aplicar migraciones y crear la base `ElBuenCorte`:
(Detener cualquier `dotnet run` antes de ejecutar)
```bash
dotnet restore
dotnet ef database update --project src\SistemaBuenCorte.DAL --startup-project src\SistemaBuenCorte.Web
```

4) Ejecutar backend (Terminal A):
```bash
dotnet run --project src\SistemaBuenCorte.Web
```

5) Ejecutar frontend (Terminal B):
```bash
cd src\SistemaBuenCorte.Web
npm install
npm start
```
O, para el sub-app `login-react-ts`:
```bash
cd src\SistemaBuenCorte.Web\login-react-ts
npm install
npm start
```

6) Credenciales de prueba (datos semilla):
- admin / Admin123!
- cajero / Cajero123!

Las contraseñas se almacenan como hashes BCrypt en la BD.

## Cómo actualizar semillas (si hace falta)
- Generar hashes BCrypt localmente y reemplazar `ContrasenaHash` en `AppDbContext.OnModelCreating`.
- Crear migración desde la raíz:
```bash
dotnet ef migrations add UpdateSeedPasswords --project src\SistemaBuenCorte.DAL --startup-project src\SistemaBuenCorte.Web
dotnet ef database update --project src\SistemaBuenCorte.DAL --startup-project src\SistemaBuenCorte.Web
```

## Autores
- Dioris Arias — @KoroDomo
- Gabriel Cuevas — @GabrielCuevas123 
- Johanny Torres — @johannytorres
- Josue David Guerrero — @guerrerodavid
- Mayerlin Alcántara — @MayiiAV

---
Programación III · ITLA · 2026