# SistemaBuenCorte
Sistema de facturación e inventario para la carnicería El Buen Corte. Proyecto de Programación III.

## Descripción

Aplicación web multicapa para la gestión de una carnicería. Permite la
autenticación de usuarios según su rol (cajero y administrador), la gestión
del inventario de productos y la facturación de ventas. El sistema persiste
los datos en una base de datos SQL Server mediante Entity Framework Core.

## Tecnologías utilizadas

- C# / .NET 8
- ASP.NET Core (Web Application)
- Entity Framework Core
- SQL Server
- Arquitectura multicapa (Presentación · Lógica de Negocio · Acceso a Datos)

## Requisitos previos

- .NET SDK 8.0 o superior
- SQL Server (LocalDB, Express o instancia local)
- Visual Studio 2022 o Visual Studio Code
- Herramienta `dotnet-ef` (`dotnet tool install --global dotnet-ef`)

## Instalación y ejecución

```bash
# 1. Clonar el repositorio
git clone https://github.com/USUARIO/SistemaBuenCorte.git
cd SistemaBuenCorte

# 2. Configurar la cadena de conexión
#    Copia tu cadena de conexión local en appsettings.Development.json
#    (no edites appsettings.json para evitar conflictos entre integrantes)

# 3. Restaurar dependencias
dotnet restore

# 4. Aplicar las migraciones (crea la base de datos y las tablas)
dotnet ef database update

# 5. Ejecutar el proyecto
dotnet run
```

> **Nota sobre la cadena de conexión:** cada integrante debe usar su propia
> instancia de SQL Server. Coloca tu cadena real en
> `appsettings.Development.json` (ignorado por git) o en User Secrets.
> El archivo `appsettings.json` versionado contiene solo un valor de ejemplo.

## Estructura del proyecto

SistemaBuenCorte/

├── SistemaBuenCorte.sln

├── src/

│   ├── SistemaBuenCorte.Web/    # Presentación (Controllers, Views, navbar/sidebar)

│   ├── SistemaBuenCorte.BLL/    # Lógica de negocio (Services)

│   └── SistemaBuenCorte.DAL/    # Acceso a datos (DbContext, Entities, Migrations)

├── README.md

└── .gitignore

## Módulos implementados

- [ ] Base de datos y configuración del repositorio
- [ ] Módulo de Login (autenticación y validación por rol)
- [ ] Módulo de Productos (CRUD con validaciones)
- [ ] Listado de productos con filtro por nombre o categoría

## Roles del sistema

- *Cajero:* acceso a facturación y operaciones de venta.
- *Administrador:* gestión de productos, inventario y acceso a reportes.

## Autores

- Dioris Arias — @usuario_github
- (Integrante 2) — @usuario_github
- (Integrante 3) — @usuario_github
- (Integrante 4) — @usuario_github
- (Integrante 5) — @usuario_github

---

Programación III · ITLA · 2026