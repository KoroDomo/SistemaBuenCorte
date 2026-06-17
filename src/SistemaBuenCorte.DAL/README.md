# Capa DAL — Acceso a Datos

Esta capa contiene todo lo relacionado con la base de datos del sistema:
las **entidades** (clases que representan tablas), el **DbContext** (puente
entre el código y SQL Server) y las **migraciones** (instrucciones para
construir la base de datos).

## Estructura

```
SistemaBuenCorte.DAL/
├── Entities/
│   ├── Usuario.cs        # Usuarios del sistema (con rol: Cajero / Administrador)
│   └── Producto.cs       # Productos del inventario (venta por Peso o Unidad)
├── Data/
│   ├── AppDbContext.cs           # DbContext: registra entidades y datos semilla
│   └── AppDbContextFactory.cs    # Permite que 'dotnet ef' funcione en esta librería
└── Migrations/           # Se genera automáticamente (no editar a mano)
```

## Cómo levantar la base de datos en tu máquina

Requisitos: **.NET 9 SDK**, **SQL Server LocalDB** y la herramienta
`dotnet-ef` (`dotnet tool install --global dotnet-ef`).

Desde la carpeta de esta capa (`src/SistemaBuenCorte.DAL`):

```bash
# Asegúrate de que LocalDB esté corriendo
sqllocaldb start MSSQLLocalDB

# Construye la base de datos a partir de las migraciones del repo
dotnet ef database update
```

Esto crea una base de datos local llamada **ElBuenCorte** con las tablas
`Usuarios` y `Productos`, ya con datos de ejemplo. No necesitas configurar
nada más: LocalDB usa una cadena de conexión idéntica para todos.

## Cómo ver las tablas (opcional)

Abre **SQL Server Management Studio (SSMS)** y conéctate al servidor:

```
(localdb)\MSSQLLocalDB
```

Verás la base de datos `ElBuenCorte` con sus tablas.

## Importante para el equipo

- **La base de datos NO se sube a GitHub.** Solo se sube el código y las
  migraciones. Cada quien construye su copia local con `dotnet ef database update`.
- Las contraseñas en los datos semilla son **placeholders**. El módulo de
  login (Punto 2) definirá el hash real y deberá regenerar esos valores.
- Si necesitas agregar o cambiar una tabla, crea una **nueva migración**
  (`dotnet ef migrations add NombreDelCambio`) — no edites las migraciones
  existentes ni la base de datos a mano.
