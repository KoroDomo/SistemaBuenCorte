# SistemaBuenCorte
Sistema de facturación e inventario para la carnicería El Buen Corte. Proyecto de Programación III.

## Descripción
Aplicación web multicapa para la gestión de una carnicería. Permite la autenticación de usuarios según su rol (cajero y administrador), la gestión del inventario de productos y la facturación de ventas. El sistema persiste los datos en una base de datos SQL Server (LocalDB) mediante Entity Framework Core. El frontend está desarrollado en React.

## Tecnologías utilizadas
- C# / .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server LocalDB
- React (Create React App)
- Axios
- Arquitectura multicapa (Presentación · Lógica de Negocio · Acceso a Datos)

## Requisitos previos
- .NET SDK 10.0 o superior
- SQL Server LocalDB (viene incluido con Visual Studio 2022)
- Visual Studio 2022
- Node.js 18 o superior
- Herramienta dotnet-ef:
```bash
dotnet tool install --global dotnet-ef
```

## Estructura del proyecto
```
SistemaBuenCorte/
├── SistemaBuenCorte.sln
├── src/
│   ├── SistemaBuenCorte.WEB/    # API (Controllers, Program.cs) + Frontend React
│   ├── SistemaBuenCorte.BLL/    # Lógica de negocio (Services, DTOs)
│   └── SistemaBuenCorte.DAL/    # Acceso a datos (DbContext, Entidades, Migraciones)
├── README.md
└── .gitignore
```

## Instalación y ejecución

### 1. Clonar el repositorio
```bash
git clone https://github.com/USUARIO/SistemaBuenCorte.git
cd SistemaBuenCorte
```

### 2. Crear la base de datos
Entra a la carpeta DAL y aplica las migraciones:
```bash
cd src/SistemaBuenCorte.DAL
dotnet ef database update
```
Esto crea la base de datos `ElBuenCorte` en tu LocalDB automáticamente. No necesitas configurar nada más.

### 3. Ejecutar el backend
Abre `SistemaBuenCorte.sln` en Visual Studio 2022 y presiona **F5** con el proyecto `SistemaBuenCorte.WEB` seleccionado.

El backend queda corriendo en `https://localhost:7299`.

### 4. Ejecutar el frontend (React)
En otra terminal, entra a la carpeta del frontend:
```bash
cd src/SistemaBuenCorte.WEB
npm install
npm start
```
El frontend abre en `http://localhost:3000`.

> **Importante:** el backend y el frontend deben estar corriendo al mismo tiempo para que el sistema funcione.

## Módulos implementados
- [x] Base de datos y migraciones (LocalDB)
- [x] Módulo de Productos — CRUD completo con validaciones
- [x] Listado de productos con búsqueda por nombre o categoría
- [ ] Módulo de Login (autenticación y validación por rol)
- [ ] Sidebar y navbar
- [ ] Módulo de Ventas
- [ ] Módulo de Caja

## Roles del sistema
- **Cajero:** acceso a facturación y operaciones de venta.
- **Administrador:** gestión de productos, inventario y acceso a reportes.

## Autores
- Dioris Arias — @usuario_github
- Gabriel Cuevas — @usuario_github
- Johanny Torres — @usuario_github
- Josue David Guerrero — @usuario_github
- Mayerlin Alcántara — @usuario_github

---
Programación III · ITLA · 2026