# SistemaBuenCorte.Web — Capa de Presentación

Interfaz de usuario del sistema, desarrollada en **React** (Create React App).

## Tecnologías

- React 18
- React Router DOM
- Axios
- Tabler Icons (webfont)

## Estructura

```
src/
├── components/
│   ├── ProductosPage.jsx       # Tabla de productos con búsqueda y paginación
│   ├── ProductosPage.css
│   ├── FormularioProducto.jsx  # Modal para crear y editar productos
│   └── FormularioProducto.css
├── services/
│   └── productosApi.js         # Funciones CRUD para conectar con la API del backend
├── mock/
│   └── productosMock.js        # Datos de ejemplo (se reemplaza cuando el backend esté listo)
└── App.js                      # Punto de entrada de la aplicación
```

## Instalación

```bash
cd src/SistemaBuenCorte.Web
npm install
npm start
```

## Módulos implementados

- Listado de productos con búsqueda por nombre o categoría
- Paginación (6 productos por página)
- Formulario modal para crear producto
- Formulario modal para editar producto
- Eliminar producto con confirmación
- Mensajes de feedback al crear, editar y eliminar

## Pendiente

- Conectar con la API real del backend (`productosApi.js` ya está preparado)
- Integrar sidebar (le corresponde a otro integrante)
- Integrar módulo de Login

## Depende de

`SistemaBuenCorte.BLL` — a través de la API REST expuesta por `SistemaBuenCorte.Web` (backend .NET)