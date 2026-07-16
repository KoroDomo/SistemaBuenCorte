# SistemaBuenCorte

Sistema web de facturación, inventario y punto de venta para la carnicería **El Buen Corte**. Proyecto de Programación III · ITLA.

Aplicación multicapa (API en .NET + frontend en React) para gestionar productos, ventas, caja, descuentos y reportes.

## Tecnologías

- C# / .NET 9 (`net9.0`) · ASP.NET Core
- Entity Framework Core 9
- SQL Server LocalDB
- React (Create React App) · Node.js
- Arquitectura N-capas: DAL (datos) · BLL (negocio) · Web (API + frontend)

---

## Cómo ejecutar el proyecto

> La base de datos **se crea y se llena con datos de prueba automáticamente** la primera vez que ejecutas el backend. **No es necesario** correr migraciones ni instalar `dotnet-ef`.

### 1. Requisitos

Instala estas tres cosas y verifica que están listas:

| Requisito | Cómo verificar | Notas |
|---|---|---|
| **.NET 9 SDK** | `dotnet --version` → debe empezar en `9.` | Si sale una versión menor (o error), instala el SDK 9 desde dotnet.microsoft.com. **Este es el error más común.** |
| **SQL Server LocalDB** | `sqllocaldb info MSSQLLocalDB` | Viene con Visual Studio o con "SQL Server Express LocalDB". Solo Windows (ver nota de Docker más abajo). |
| **Node.js 18+** | `node --version` | Incluye `npm`. |

### 2. Traer el código más reciente

Todo el proyecto integrado vive en la rama **`main`**:

```bash
git clone https://github.com/KoroDomo/SistemaBuenCorte.git
cd SistemaBuenCorte
```

Si ya lo tenías clonado:

```bash
git checkout main
git pull origin main
```

### 3. Ejecutar el backend (Terminal 1)

Desde la raíz del repositorio:

```bash
dotnet run --project src/SistemaBuenCorte.Web
```

La primera vez, esto crea la base `ElBuenCorte` en LocalDB y la siembra con los usuarios y datos de prueba. Cuando veas `Now listening on: http://localhost:5097`, el backend está listo. **Déjalo corriendo.**

### 4. Ejecutar el frontend (Terminal 2)

En una **segunda** terminal:

```bash
cd src/SistemaBuenCorte.Web
npm install
npm start
```

Se abre el navegador en `http://localhost:3000`. El frontend habla con el backend en el puerto 5097, así que **ambas terminales deben estar corriendo a la vez.**

### 5. Iniciar sesión (credenciales de prueba)

| Usuario | Contraseña | Rol |
|---|---|---|
| `admin` | `Admin123!` | Administrador |
| `cajero` | `Cajero123!` | Cajero |

---

## Solución de problemas comunes

**El login dice "contraseña incorrecta" o similar.**
Casi siempre significa que la base de datos no se creó/sembró. Asegúrate de haber ejecutado `dotnet run` al menos una vez (crea y siembra la BD sola) y de que LocalDB esté disponible. Si el backend no arrancó por falta de .NET 9, la BD nunca se crea.

**`dotnet` no se reconoce, o la versión no es 9.x.**
Falta el SDK de .NET 9. Instálalo y reinicia la terminal. Verifica con `dotnet --version`.

**No conecta a la base de datos / error de LocalDB.**
LocalDB no está instalado o no está corriendo. En Windows:
```powershell
sqllocaldb start MSSQLLocalDB
```
(En Windows normalmente arranca solo al conectarse.)

**El puerto 5097 o 3000 está ocupado.**
Hay una instancia anterior corriendo. Ciérrala (Ctrl+C en su terminal) o cierra el proceso que usa el puerto.

**El frontend abre pero no carga datos / errores en consola.**
El backend no está corriendo, o está en otro puerto. Confirma que la Terminal 1 muestra `Now listening on: http://localhost:5097`.

**Ya había corrido `dotnet ef database update` antes y ahora algo falla.**
El proyecto crea la BD con `EnsureCreated`, no con migraciones. Si tienes una BD en estado inconsistente, elimínala (base `ElBuenCorte` en LocalDB) y deja que `dotnet run` la vuelva a crear desde cero.

**Git en Windows da errores por rutas largas.**
```powershell
git config --system core.longpaths true
```

**Usar Docker en vez de LocalDB (Mac/Linux o quien no tenga LocalDB).**
LocalDB es solo para Windows. En otros sistemas, levanta SQL Server en Docker y cambia la cadena `DefaultConnection` en `src/SistemaBuenCorte.Web/appsettings.json` para apuntar a ese contenedor.

---

## Estructura del proyecto

```
SistemaBuenCorte/
├── SistemaBuenCorte.sln
├── src/
│   ├── SistemaBuenCorte.Web/    # API, controllers y frontend React (carpeta src/)
│   ├── SistemaBuenCorte.BLL/    # Lógica de negocio (servicios, DTOs, validaciones)
│   └── SistemaBuenCorte.DAL/    # DbContext, entidades y migraciones
└── tests/
    └── SistemaBuenCorte.Tests/  # Pruebas unitarias (xUnit)
```

## Módulos

- **Productos** — CRUD de productos con control de inventario.
- **Ventas / Punto de venta** — registro de ventas, descuento de stock y generación de factura.
- **Caja** — apertura y cierre de turnos de caja.
- **Descuentos** — catálogo de descuentos (porcentaje o monto fijo) gestionado por el administrador.
- **Reportes** — reportes de ventas para administración.

## Ejecutar las pruebas

```bash
dotnet test
```

## Autores

- Dioris Arias — @KoroDomo
- Gabriel Cuevas — @GabrielCuevas123
- Johanny Torres — @johannytorres
- Josue David Guerrero — @guerrerodavid
- Mayerlin Alcántara — @MayiiAV

---

Programación III · ITLA · 2026
